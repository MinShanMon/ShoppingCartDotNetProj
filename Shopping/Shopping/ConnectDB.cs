using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        public bool RemoveUserCart(int productId, int quantity)
        {
            bool sucessRemove = false;
            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                connect.Open();
                string query = String.Format(@"Delete From UserCart Where ProductId = '{0}' and Quantity = '{1}'
                 )", productId, quantity);

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        sucessRemove = true;
                    }
                    connect.Close();
                }
                return sucessRemove;
            }
        }

        public bool AddUserCart(int userId, int productId, int quantity)
        {
            bool sucessAdd = false;
            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                connect.Open();
                string query = String.Format(@"INSERT INTO UserCart(UserId, ProductId, Quantity) VALUES
                 ('{0}','{1}','{2}')", userId, productId, quantity);

                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        sucessAdd = true;
                    }
                    connect.Close();
                }
                return sucessAdd;
            }
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
                        ProductPrice = (decimal)reader["ProductPrice"]
                    };
                    products.Add(product);
                }
            }
            return products;
        }

        public List<Product> RetrieveProduct(string pids)
        {
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
            }
            return products;
        }

        public List<Product> SearchProduct(string input)
        {
            List<Product> products = new List<Product> { };
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
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
                        ProductPrice = (decimal)reader["ProductPrice"]
                    };
                    products.Add(product);
                }
            }
            return products;
        }

        //public List<PurchasedList> PurchasedList(int userId)
        //{
        //    List<PurchasedList> PurchasedLists = new List<PurchasedList> { };
        //    using (SqlConnection conn = new SqlConnection(this.connectionString))
        //    {
        //        conn.Open();
        //        string sql = @"Select p.ProductId,p.ProductName,p.ProductDesc,p.ProductImg,p.ProductPrice,ac.ActivationCode,od.Quantity,o.Timestamp
        //                        From OrderDetails od
        //                        inner join Products p
        //                        On od.ProductId =p.ProductId
        //                        inner join ActivationCodeDetails ac
        //                        On od.OrderId =ac.OrderId and od.ProductId=ac.ProductId
        //                        inner join Orders o
        //                        ON o.OrderId=od.OrderId
        //                        where UserId=" + userId;
        //        SqlCommand cmd = new SqlCommand(sql, conn);

        //        SqlDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            int productId = (int)reader["ProductId"];
        //            PurchasedList PurchasedList = new PurchasedList()
        //            {
        //                ProductId = (int)reader["ProductId"],
        //                ProductName = (string)reader["ProductName"],
        //                ProductDesc = (string)reader["ProductDesc"],
        //                ProductImg = (string)reader["ProductImg"],
        //                ProductPrice = (decimal)reader["ProductPrice"],
        //                ActivationCode = new List<Guid> { (Guid)reader["ActivationCode"] },
        //                Timestamp = (long)reader["Timestamp"],
        //                Qty = (int)reader["Quantity"],
        //            };
        //            bool flag = false;
        //            foreach (PurchasedList passPurchasedList in PurchasedLists)
        //            {
        //                if (passPurchasedList.ProductId == PurchasedList.ProductId && passPurchasedList.Timestamp == PurchasedList.Timestamp)
        //                {
        //                    passPurchasedList.ActivationCode.Add(PurchasedList.ActivationCode[0]);
        //                    flag = true;
        //                    break;
        //                }
        //            }
        //            if (!flag)
        //            {
        //                PurchasedLists.Add(PurchasedList);
        //            }

        //        }
        //    }
        //    return PurchasedLists;
        //}

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public bool AddOrder(int userid, List<CartDetail> cartDetails)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {

                conn.Open();
                string sql = String.Format(@"INSERT INTO Orders (UserId, Timestamp)
                                          VALUES ({0}, '{1}')", userid, GetTimestamp(DateTime.Now));


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
                }
                
                return true;
            }
        }

        public List<PurchasedList> RetrievePurchase(int userId)
        {
            List<PurchasedList> products = new List<PurchasedList>();
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                string sql = string.Format("select O.Timestamp, p.ProductId, p.ProductPrice, od.Quantity, p.productname, p.[ProductImg]," +
                    " p.productdesc from Orders o inner join OrderDetails od on od.OrderId = o.OrderId inner join Products p on p.ProductId = " +
                    "od.ProductId where o.UserId = {0}", userId);

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    PurchasedList product = new PurchasedList()
                    {
                        ProductId = (int)reader["ProductId"],
                        ProductName = (string)reader["ProductName"],
                        ProductDesc = (string)reader["ProductDesc"],
                        ProductImg = (string)reader["ProductImg"],
                        ProductPrice = (decimal)reader["ProductPrice"],
                        TimeStamp = (Int64)reader["Timestamp"],
                        Qty = (int)reader["Quantity"]
                    };
                    products.Add(product);
                }
                conn.Close();

            }
            return products;
        }


        public List<PurchasedActivation> RetrieveActivations(int userId)
        {
            List<PurchasedActivation> purchaseds = new List<PurchasedActivation>();
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            {
                conn.Open();
                string sql = string.Format("select a.ActivationCode, p.productId from Products p " +
                    "inner join ActivationCodeDetails a on p.ProductId = a.ProductId inner join Orders o on a.OrderId = o.OrderId " +
                    "inner join Users c on o.UserId = c.UserId where c.UserId = {0}", userId);
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PurchasedActivation purchased = new PurchasedActivation()
                    {
                        ActivationCode = (Guid)reader["ActivationCode"],
                        ProductId = (int)reader["productId"]
                    };
                    purchaseds.Add(purchased);

                }
                conn.Close();
            }
            return purchaseds;
        }

        public bool SetStar(int cid, int pid, int rid)
        {
            return UpdateStar(cid, pid, rid);
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


