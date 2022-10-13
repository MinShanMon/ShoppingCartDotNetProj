window.onload = function () {
    let checkoutbtn = document.getElementById("checkoutbtn");
    checkoutbtn.onclick = Checkout();
}

function Checkout() {
    let cart = JSON.parse(localStorage["cart"]);
    let list = [];
    for (const entry in cart) {
        if (entry == "totalQuantity") {
            continue;
        }
        if (cart[entry] == 0) {
            continue;
        }
        let order = {ProductId:entry,Quantity:cart[entry]};
        list.push(order);
    }
    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/Checkout/Index");
    xhr.setRequestHeader("Content-Type", "application/json; charset=utf8");
    let data = JSON.stringify(list);
    xhr.send(data);
   
}

