
$(document).ready(function () {

    $(".container-flud").LoadingOverlay("show");

    fetch("/Home/ObtenerUsuario").then(response => {
        $(".container-flud").LoadingOverlay("hide");
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        if (responseJson.estado) {

            console.log(responseJson);
            const d = responseJson.object;

            $("#imgFoto").attr("src", d.urlFoto);

            $("#txtNombre").val(d.nombre);
            $("#txtCorreo").val(d.correo);
            $("#txTelefono").val(d.telefono);
            $("#txtRol").val(d.nombreRol);
        } else {
            swal("Lo Sentimos", responseJson.message, "error");
        }
    })
});


$("#btnGuardarCambios").click(function () {
    if ($("#txtCorreo").val().trim() == "") {
        toastr.warning("", "Debe completar el campo: Correo");
        $("#txtCorreo").focus();
        return;
    }
    if ($("#txTelefono").val().trim() == "") {
        toastr.warning("", "Debe completar el campo: Telefono");
        $("#txTelefono").focus();
        return;
    }


    swal({
        title: "¿Desea guardar los cambios?",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-primary",
        confirmButtonText: "Si",
        cancelButtonText: "No",
        closeOnConfirm: false,
        closeOnCancel: true
    }, function (respuesta) {
        if (respuesta) {
            $(".showSweetAlert").LoadingOverlay("show");

            let modelo = {
                correo: $("#txtCorreo").val(),
                telefono: $("#txTelefono").val()
                }

            fetch(`/Home/GuardarPerfil`, {
                method: "POST",
                headers: { "Content-type": "application/json; charset=utf-8" },
                body: JSON.stringify(modelo)
            }).then(response => {
                $(".showSweetAlert").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            }).then(responseJson => {
                if (responseJson.estado) {
                    swal("Listo!", "los cambios fueron guardados", "success");
                } else {
                    swal("Lo Sentimos", responseJson.message, "error");
                }
            })

        }
    }
    );


})


$("#btnCambiarClave").click(function () {
    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debe completar el campo: "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje);

        $(`input[name="${inputs_sin_valor[0].name}]"`).focus();
        return;
    }

    if ($("#txtClaveNueva").val().trim() != $("#txtConfirmarClave").val().trim()) {
        toastr.warning("", "Las contraseñas ingresadas no coinciden");
        $("#txtClaveNueva").focus();
        return;
    }


    let modelo = {
        claveActual: $("#txtClaveActual").val().trim(),
        claveNueva: $("#txtClaveNueva").val().trim()
        }

    fetch("/Home/CambiarClave", {
        method: "POST",
        headers: { "Content-type": "application/json; charset=utf-8" },
        body: JSON.stringify(modelo)
    }).then(response => {
        $(".showSweetAlert").LoadingOverlay("hide");
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        if (responseJson.estado) {
           
            swal("Listo!", "Su contraseña Fue Actualizada", "success");
            $("input.input-validar").val("")

        } else {
            swal("Lo Sentimos", responseJson.message, "error");
        }
    })

})