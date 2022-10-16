window.onload = function () {
    getLastQty();
    setProductIds();
}



function search() {
    let str = document.getElementById("search_input");
    window.location.replace("/home/?search=" + str.value);
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
    document.cookie = "items=" + encodeURIComponent(productids) + ";" + "path=/";
}


function addToCart(event) {   
    let productId = event.srcElement.parentNode.id;

    //get cart data from localStorage, if it is empty, do the initialisation
    if (localStorage["cart"] == null) {
        localStorage["cart"] = "{}";
    }
    let cartStr = localStorage["cart"];

    //convert from  JSON str to js object
    let cart = JSON.parse(cartStr);
    //if the product has not been added before, set value=1 (amount) for this product, if added,increase the amount by 1
    if (cart[productId] == null) {
        cart[productId] = 1;
    } else {
        cart[productId] += 1;
    }

    //increase total quatity
    if (cart["totalQuantity"] == null) {
        cart["totalQuantity"] = 1;
    } else {
        cart["totalQuantity"] += 1;
    }
    
    //modify the number of cart
    let totalNum = document.getElementById("totalNum");
    totalNum.childNodes[0].data = cart["totalQuantity"];

    //store it back to localStorage
    localStorage["cart"] = JSON.stringify(cart);

    //update the change to DB if user has logined
    if (localStorage["sessionId"] != null) {
        let userId = localStorage["sessionId"];
        addToCartDB(sessionId, productId);
    }
    setProductIds();
}

//get cartqty from localStorage
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

//make a AJAX request when user has logined
function addToCartDB(sessionId,productId) {
    let xhr = new XMLHttpRequest();

    xhr.open("POST", "/Home/AddToCart");
    xhr.setRequestHeader("Content-Type", "application/json");

    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            if (this.status !== 200) {
                return;
            }
            let data = JSON.parse(this.responseText);
            if (data.isOkay === false) {
                alert("Userid is invalid.")
            }
        }
    }
    let cart = {
        "sessionId": sessionId,
        "productId": productId
    };
    xhr.send(JSON.stringify(cart));
}