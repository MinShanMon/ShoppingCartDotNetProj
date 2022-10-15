/*window.onload = function () {
    setProductIds();
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
    document.cookie = "items=" + encodeURIComponent(productids) + ";" + "paths=/;";
}*/

