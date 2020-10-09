$(".btnComida").click(function (eve) {
    //alert("btnComida");
    $("#modal-content").load("/Comida/VerComida/" + $(this).data("id"));
});

$(".btnMostrarCarrito").click(function (eve) {
    //alert("btnMostrarCarrito");
    $("#modalCarrito-content").load("/Comida/MostrarCarrito/");
});

$(".numbers").keydown(function (eve) {
    //alert("ENTROOOOOOOOOOOOOOOOOO");
});

$(".btncargaCarrito").click(function (eve) {
    //alert("btncargaCarrito");
    $("#modalCarrito-content").load("/Comida/MostrarCarrito/");
});

