window.onload = function () {
    let elems = document.getElementsByClassName("increment");
    for (i = 0; i < elems.length; i++) {
        elems[i].addEventListener('click', Increment)
    }
    elems = document.getElementsByClassName("decrement");
    for (i = 0; i < elems.length; i++) {
        elems[i].addEventListener('click', Decrement)
    }
    getLastQty();
    isCartEmpty();
    DisplayCartAndPrice();
    setProductIds();
    let checkoutbtn = document.getElementById("checkoutbtn");
    checkoutbtn.addEventListener('click', Checkout);
    removeItem();
    setRedirectAction()
}

function setRedirectAction() {
    if (getCookie("SessionId") == null) {
        document.getElementById("isLogin").action = "/Login/isLogin";
    }
    else {
        document.getElementById("checkoutbtn").type = "button";
    }
}

function getLastQty() {
    let totalNum = document.getElementById("totalNum");
    if (localStorage["cart"] == null) {
        localStorage["cart"] = "{}";
    }
    let cartStr = localStorage["cart"];
    let cart = JSON.parse(cartStr);
    if (cart["totalQuantity"] == null) {
        cart["totalQuantity"] = 0;
    }
    totalNum.childNodes[0].data = cart["totalQuantity"];
}


function isCartEmpty() {
    let cart = JSON.parse(localStorage["cart"]);
    if (!cart || cart["totalQuantity"] == 0) {
        document.getElementById("body").innerHTML = "The cart is empty!";
        return;
    };
}

function DisplayCartAndPrice() {
    let cart = JSON.parse(localStorage["cart"]);
    let totalprice = 0;
    for (const key in cart) {
        if (key == "totalQuantity") {
            continue;
        }
        let quantityelem = document.getElementById(+key);
        quantityelem.innerHTML = cart[key];
        let priceelem = document.getElementById("price " + key)
        let eachprice = parseInt(priceelem.innerHTML);
        totalprice = totalprice + eachprice * cart[key];
    }
    let totalpriceelem = document.getElementById("totalprice");
    totalpriceelem.innerHTML = "$" + totalprice;
}

function setLastQty() {
    let totalNum = document.getElementById("totalNum");
    if (localStorage["cart"] == null) {
        localStorage["cart"] = "{}";
    }
    let cart = JSON.parse(localStorage["cart"]);
    if (cart["totalQuantity"] == null) {
        cart["totalQuantity"] = 0;
    }
    totalNum.childNodes[0].data = cart["totalQuantity"];
}

function setProductIds() {
    let cart = JSON.parse(localStorage["cart"])
    let productids = [];
    for (const productid in cart) {
        if (productid == "totalQuantity") {
            continue;
        }
        productids.push(productid);
    }
    document.cookie = "items=" + encodeURIComponent(productids) + ";" + "paths=/";
}


function Increment(event) {
    let span = event.target.nextElementSibling;
    span.innerHTML++;
    let cart = JSON.parse(localStorage["cart"]);
    cart["totalQuantity"]++;
    if (cart[span.id] == null) {
        cart[span.id] = 1;
    } else {
        cart[span.id]++;
    }
    localStorage["cart"] = JSON.stringify(cart);
    setProductIds()
    getLastQty();
    document.getElementById("totalprice").innerHTML = "$" + GetTotalPrice(span, 1);
}

function Decrement(event) {
    let span = event.target.previousElementSibling;
    if (span.innerHTML == 1) {
        return;
    }
    span.innerHTML--;
    let cart = JSON.parse(localStorage["cart"]);
    cart["totalQuantity"]--;

    cart[span.id]--;

    localStorage["cart"] = JSON.stringify(cart);
    setProductIds();
    getLastQty();
    document.getElementById("totalprice").innerHTML = "$" + GetTotalPrice(span, -1);
}


function GetTotalPrice(elem, value) {
    let quantityelem = document.getElementById(elem.id);
    let priceelem = document.getElementById("price " + elem.id)
    let eachprice = parseInt(priceelem.innerHTML);
    let totalprice = parseInt(document.getElementById("totalprice").innerHTML.substring(1)) + (eachprice * value);
    return totalprice;
}



function Checkout() {
    if (getCookie("SessionId") == null) {
        return;
    }

    let cart = JSON.parse(localStorage["cart"]);
    let list = [];
    for (const entry in cart) {
        if (entry == "totalQuantity") {
            continue;
        }
        if (cart[entry] == 0) {
            continue;
        }
        let order = { ProductId: entry, Quantity: cart[entry] };
        list.push(order);
    }
    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/Checkout/Index/");
    xhr.setRequestHeader("Content-Type", "application/json; charset=utf8");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            if (this.status !== 200) {
                return;
            }

            let data = JSON.parse(this.responseText);
            if (data.isOkay === false) {
                alert("Error. Please try again.")
            }
            document.location = "/PurchaseList";
        }
    }

    let data = JSON.stringify(list);
    xhr.send(data);
    localStorage.clear();
    document.cookie = "items = ;expires=Thu, 01 Jan 1970 00:00:00 UTC; ";
}

function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return null;
}


function removeItem() {
    var removeCartItemButtons = document.getElementsByClassName('text-danger');
    for (i = 0; i < removeCartItemButtons.length; i++) {
        var button = removeCartItemButtons[i];
        button.addEventListener('click', removeCartItem);
    }
}


function removeCartItem(event) {
    let id = event.target.parentElement.id.substring(2);
    let cart = JSON.parse(localStorage["cart"]);
    var pid = parseInt(id);
    let amount = cart[pid];
    console.log(amount);
    cart["totalQuantity"] = cart["totalQuantity"] - amount;
    let priceelem = document.getElementById("price " + pid)
    let eachprice = parseInt(priceelem.innerHTML);
    let removeValue = eachprice * amount;
    var beforeTotal = parseInt(document.getElementById("totalprice").innerHTML.substring(1));
    var remainTotal = beforeTotal - removeValue;
    document.getElementById("totalprice").innerHTML = "$" + remainTotal;
    console.log(beforeTotal);
    delete cart[pid];
    localStorage["cart"] = JSON.stringify(cart);
    getLastQty();
    setProductIds();
    if (cart["totalQuantity"] == 0) {
        delete cart["totalQuantity"];
        location.reload();

    }
    localStorage["cart"] = JSON.stringify(cart);
    event.target.parentNode.parentElement.parentElement.parentElement.parentElement.remove();

}
