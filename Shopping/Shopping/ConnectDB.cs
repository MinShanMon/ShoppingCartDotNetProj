using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using Shopping.Models;

namespace Shopping
{
    public class ConnectDB
    {

        private string connectionString;

        public ConnectDB(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public string AddSession(int userId)
        {
            string sessionId = null;
            Guid guid = Guid.NewGuid();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = String.Format(@"INSERT INTO Sessions(
                SessionId, UserId) VALUES('{0}', '{1}')",
                        guid, userId, DateTimeOffset.Now.ToUnixTimeSeconds());

                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        sessionId = guid.ToString();
                    }
                }

                conn.Close();
            }

            return sessionId;
        }




        public User GetUserByUsername(string username)
        {
            User user = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = String.Format(@"SELECT * FROM [Users] 
                WHERE Username = '{0}'", username);

                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserId = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2)
                            };
                        }
                    }
                }

                conn.Close();
            }

            return user;
        }




        public bool RemoveSession(string sessionId)
        {
            bool status = false;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = String.Format(@"DELETE FROM Sessions
                WHERE sessionId = '{0}'", sessionId);

                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        status = true;
                    }
                }

                conn.Close();
            }

            return status;
        }



        public User GetUserBySession(string sessionId)
        {
            User user = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = String.Format(@"SELECT u.UserId, Username, Password
             FROM Sessions s, [Users] u
                WHERE s.UserId = u.UserId AND 
                    s.SessionId = '{0}'", sessionId);

                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserId = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2)
                            };
                        }
                    }
                }

                conn.Close();
            }

            return user;
        }

       

       
        public List<Product> RetrieveProduct()
        {
            List<Product> products = new List<Product> { };
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();



                string sql = @"select * from products";
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Product product = new Product()
                    {
                        ProductId = (int)reader["ProductId"],
                        ProductName = (string)reader["ProductName"],
                        ProductDesc = (string)reader["ProductDesc"],
                        ProductImg = (string)reader["ProductImg"],
                        ProductPrice = (decimal)reader["ProductPrice"],
                        ProductRating = rating((int)reader["ProductId"])


                    };
                    products.Add(product);
                }

                conn.Close();
            }
            return products;
        }


     

        public int rating(int productid)
        {


            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();

                string sql2 = String.Format(@"select avg(Rating) as Rating from Reviews join Users on Reviews.UserId = Users.UserId where Reviews.ProductId = {0}", productid);
                SqlCommand cmd = new SqlCommand(sql2, conn);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    try
                    {
                        return reader.GetInt32(0);
                    }
                    catch (System.Data.SqlTypes.SqlNullValueException)
                    {
                        return 0;
                    }
                    return 0;
                }

                conn.Close();
            }
            return 0;
        }



        public List<Product> RetrieveProduct(string pids)
        {

            if(pids == "")
            {
                return null;
            }
            List<Product> products = new List<Product> { };
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                string sql = String.Format(@"select * from products where productid in ({0})", pids);
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Product product = new Product()
                    {
                        ProductId = (int)reader["ProductId"],
                        ProductName = (string)reader["ProductName"],
                        ProductDesc = (string)reader["ProductDesc"],
                        ProductImg = (string)reader["ProductImg"],
                        ProductPrice = (decimal)reader["ProductPrice"]
                    };
                    products.Add(product);
                }
                conn.Close();
            }
            return products;
        }



        public List<Product> SearchProduct(string input)
        {
            List<Product> products = new List<Product> { };
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                input = input.Replace("'", "");
                string sql = @"select * from products where ProductName like '%"+input +"%'or ProductDesc like '%"+ input+"%'";

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Product product = new Product()
                    {
                        ProductId = (int)reader["ProductId"],
                        ProductName = (string)reader["ProductName"],
                        ProductDesc = (string)reader["ProductDesc"],
                        ProductImg = (string)reader["ProductImg"],
                        ProductPrice = (decimal)reader["ProductPrice"],
                        ProductRating = rating((int)reader["ProductId"])
                    };
                    products.Add(product);
                }
                conn.Close();
            }
            return products;
        }

      


        public bool AddOrder(int userid, List<CartDetail> cartDetails)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {

                conn.Open();
                DateTime date = DateTime.Now;
                string dateString = date.Year + "-" + date.Month + "-" + date.Day;
                string sql = String.Format(@"INSERT INTO Orders (UserId, Timestamp)
                                          VALUES ({0}, '{1}')", userid, dateString);


                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();


                string sqlid = String.Format(@"SELECT TOP 1 OrderId FROM Orders
                                            WHERE UserId = {0}
                                            ORDER BY OrderId desc", userid);

                cmd = new SqlCommand(sqlid, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                int orderId = reader.GetInt32(0);
                conn.Close();

                conn.Open();

                foreach (CartDetail cartDetail in cartDetails)
                {

                    string sql2 = String.Format(@"INSERT INTO OrderDetails
                                            VALUES ({0}, {1}, {2})", orderId, cartDetail.ProductId, cartDetail.Quantity);
                    cmd = new SqlCommand(sql2, conn);
                    cmd.ExecuteNonQuery();


                    for (int i = 0; i < cartDetail.Quantity; i++)
                    {
                        string sql3 = String.Format(@"INSERT INTO ActivationCodeDetails
                                            VALUES ('{0}', {1}, {2})", Guid.NewGuid(), cartDetail.ProductId, orderId);
                        cmd = new SqlCommand(sql3, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                
                return true;

                conn.Close();
            }
        }

        public List<string> RetrieveDate(int userId)
        {
            List<string> dateList = new List<string>();
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                string sql = string.Format("select distinct([Timestamp]) as date from orders where UserId = {0};",userId);
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dateList.Add((string)reader["date"].ToString());
                }

                conn.Close();
            }
            return dateList;
        }


        public List<PurchasedList> RetrievePurchase(int userId)
        {
            List<PurchasedList> products = new List<PurchasedList>();
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();

                List<string> dateList= RetrieveDate(userId);

                foreach(string date in dateList)
                { 
                string sql = string.Format("select O.Timestamp, p.ProductId, p.ProductPrice, od.Quantity, p.productname, p.[ProductImg]," +
                    " p.productdesc from Orders o inner join OrderDetails od on od.OrderId = o.OrderId inner join Products p on p.ProductId = " +
                    "od.ProductId where o.UserId = {0} and O.Timestamp = '{1}'", userId, date);

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                List<int> productids = new List<int>();
                    while (reader.Read())
                    {
                        int currproductid = (int)reader["ProductId"];
                        if (productids.Contains(currproductid))
                        {
                            foreach (PurchasedList pdt in products)
                            {
                                if (pdt.ProductId == currproductid)
                                {
                                    pdt.Qty += (int)reader["Quantity"];
                                    break;
                                }
                            }
                        }
                        else
                        {
                            PurchasedList product = new PurchasedList()
                            {
                                ProductId = (int)reader["ProductId"],
                                ProductName = (string)reader["ProductName"],
                                ProductDesc = (string)reader["ProductDesc"],
                                ProductImg = (string)reader["ProductImg"],
                                ProductPrice = (decimal)reader["ProductPrice"],
                                TimeStamp = reader.GetDateTime(0).ToString("D"),
                                Qty = (int)reader["Quantity"],
                            };
                            products.Add(product);
                            productids.Add(currproductid);
                        }
                    }
                   
                }
                conn.Close();

            }
            return products;
        }


        public string datetimeforActivition(int orderId)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();

                string sql = string.Format("select [Timestamp] as date from" +
                " Orders o inner join OrderDetails od on od.OrderId = o.OrderId " +
                "inner join Products p on p.ProductId = od.ProductId where o.OrderId = {0}", orderId);
                 SqlCommand cmd = new SqlCommand(sql, conn);
                 SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                    return reader.GetDateTime(0).ToString("D");
                    }
                
                conn.Close();
            }
            return "";
        }
    

        public List<PurchasedActivation> RetrieveActivations(int userId)
        {
            List<PurchasedActivation> purchaseds = new List<PurchasedActivation>();
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();

                List<string> dateList = RetrieveDate(userId);

                foreach (string date in dateList)
                {
                    string sql = string.Format("select * from ActivationCodeDetails where orderid in " +
                    "(select o.OrderId from Orders o inner join OrderDetails od on od.OrderId = o.OrderId " +
                    "inner join Products p on p.ProductId = od.ProductId " +
                    "where o.UserId = {0} and O.[Timestamp] = '{1}')", userId, date);
                    SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        PurchasedActivation purchased = new PurchasedActivation()
                        {
                            ActivationCode = (Guid)reader["ActivationCode"],
                            ProductId = (int)reader["productId"],
                            TimeStamp = (string)datetimeforActivition((int)reader["OrderId"]),
                        };
                        purchaseds.Add(purchased);
                    }
                }
                conn.Close();
            }
            return purchaseds;
        }



        public bool RatingIsExist(int cid, int pid, int rid)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                string sql = String.Format(@"Select * From Reviews where UserId = {0} and ProductId = {1}", cid, pid);
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            reader.GetInt32(0);
                            return UpdateStar(cid, pid, rid);
                        }
                        else
                        {
                            return ReviewProduct(cid, pid, rid);
                        }
                    }
                }
                conn.Close();
            }
        }


        public bool ReviewProduct(int userId, int ProductId, int Rating)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Reviews
                            VALUES (" + userId + "," + ProductId + "," + Rating + ")";

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                conn.Close();
            }
        }

        public bool UpdateStar(int cid, int pid, int rid)
        {
            bool status = false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string q = String.Format(@"update Reviews set rating = {0} where UserId = {1} and ProductId = {2}", rid, cid, pid);
                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        status = true;
                    }
                }
                conn.Close();
            }
            return status;
        }


        public List<Reviews> GetStar(int UserId)
        {
            List<Reviews> stars = new List<Reviews>();


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string q = String.Format("SELECT * FROM Reviews where UserId = {0}", UserId);

                using (SqlCommand cmd = new SqlCommand(q, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Reviews star = new Reviews
                            {
                                UserId = reader.GetInt32(0),
                                ProductId = reader.GetInt32(1),
                                Rating = reader.GetInt32(2)
                            };
                            stars.Add(star);

                        }
                    }
                }
                conn.Close();
            }
            return stars;
        }
    }

}


