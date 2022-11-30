// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.onload = function () {
    dbGetStar();
    ready();
}

function ready() {

    let star = document.getElementsByClassName("star");
    for (let i = 0; i < star.length; i++) {
        star[i].addEventListener('click', onStarClick);
    }
}



function dbrestore() {
    dbGetStar();
}

function dbGetStar() {
    let ajax = new XMLHttpRequest();
    ajax.open("GET", "/PurchaseList/GetStar");

    ajax.onreadystatechange = function () {
        if (this.readyState == XMLHttpRequest.DONE) {
            if (this.status == 200) {
                onGetStar(JSON.parse(this.responseText));
            }
        }
    }
    ajax.send();
}

function onGetStar(data) {
    if (data == null) {
        return;
    }
    for (let i = 0; i < data.length; i++) {
        let entry = data[i];
        let inputStar = document.getElementsByClassName("star_" + data[i].ProductId);
        for (let j = 0; j < inputStar.length; j++) {
            if (inputStar[j].id == data[i].Rating + "l") {
                inputStar[j].classList.add('selected');
            }
        }
    }
}

function onStarClick(event) {
    var id = event.target.id;
    var clas = document.getElementsByClassName("star_" + event.target.value);
    for (let i = 0; i < clas.length; i++) {
        clas[i].classList.remove('selected');
        //location.reload();
    }
    event.target.parentElement.classList.add('selected');
    SetStarRating(event.target.value, id);
   
}



function SetStarRating(ProductId, Rating) {
    let ajax = new XMLHttpRequest();
    ajax.open("POST", "/PurchaseList/SetStarRating");
    ajax.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

    ajax.onreadystatechange = function () {
        if (this.readyState == XMLHttpRequest.DONE) {
            if (this.status == 200) {
                return this.responseText;
            }
        }
    }
    ajax.send("productId=" + ProductId + "&rating=" + Rating);
}



