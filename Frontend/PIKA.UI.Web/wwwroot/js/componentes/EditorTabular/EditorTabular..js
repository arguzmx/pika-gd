//El editor tabular se encarga de realizar las operaciones
//de CRUD para el tipo de entidad exietente en el endpoint de creación
class editorTabular {

    constructor(url) {
        this.url = url;
        this.client = new ClientAPI(url);
        this.table = "";
    }

    //Punto de entrada para el editor
    inicializarEditorTabular() {
        this.client.getMetadata().then((respuesta) => {
            if (!respuesta.es_error) {
                console.log(respuesta.datos);

                this.table = this.generaTabla(respuesta.datos.Propiedades, 
                    "http://localhost:5000/api/v1.0/org/Dominio/datatables");

            } else {
                ///
            }
        });
    }

    generaTabla(propiedades, url) {
        let tableHeaders = [];
        let columnDefs = [];
        let visible = false;
        let ordenable = false;
        let buscable = false;

        propiedades.sort(function (a, b) {
            return a.IndiceOrdenamiento - b.IndiceOrdenamiento;
        });

        for (var i = 0; i < propiedades.length; i++) {
            if (propiedades[i].AtributoTabla !== null) {

                visible = propiedades[i].AtributoTabla.Visible === true ? true : false;
                ordenable = propiedades[i].Ordenable === true ? true : false;

                tableHeaders.push({ data: propiedades[i].Nombre, title: propiedades[i].Nombre });
                columnDefs.push({ targets: i, visible: visible, orderable: ordenable });

                //render para checkbox
                if (propiedades[i].TipoDatoId === "bool") {
                    columnDefs[columnDefs.length - 1].render = function (data, type, row) {
                        if (type === 'display') {
                            let checked = data === true ? "checked" : "";
                            let divCheckBox = "<div class='icheck-primary d-inline'>"
                                + "<input type = 'checkbox' id='eliminada[" + row.Id + "]' disabled " + checked + "/>"
                                + "<label for='eliminada[" + row.Id + "]'></label>"
                                + ""
                                + "</div> ";

                            return divCheckBox;
                        }
                        return data;
                    };

                    columnDefs[columnDefs.length - 1].className = "dt-center";
                }
            }
        }
        let t = $('#tablaeditortabular').DataTable({
            select: {
                style: 'multi'
            },
            columns: tableHeaders,
            columnDefs: columnDefs,
            order: [1, "asc"],
            "serverSide": true,
            "ajax": {
                "url": url,
                'headers': {
                    'Access-Control-Allow-Origin': '*',
                    'Content-Type': 'application/json',
                    'uid': this.client.authInfo.uid,
                    'Authorization': `Bearer ${this.client.authInfo.jwt}`
                }
            }
        });
        return t;
    }
    //***Pruebas para dominio ***///
    obtieneDatosForm(propiedades, formulario) {


    }

    post_put(data, operacion) {
        //encodeURIComponent(JSON.stringify(myArray));
        let id = data.Id;
        console.log(data);
        if (operacion === "delete") {

            for (var i = 0; i < data.id.length; i++) {
                id += "id=" + data.id[i]+"&";
            }
            id = id.slice(0, -1);
            id = jQuery.param(id);
            //id = encodeURIComponent(JSON.stringify(id));
        }
        console.log(id);

        $.ajax({
            url: "http://localhost:5000/api/v1.0/org/Dominio/" + id,
            type: operacion,
            dataType: "json",
            contentType: 'application/json',
            data: JSON.stringify(data),
            headers: {
                'Access-Control-Allow-Origin': '*',
                'Content-Type': 'application/json',
                'uid': this.client.authInfo.uid,
                'Authorization': `Bearer ${this.client.authInfo.jwt}`
            },
            success: function (data, status) {
                if (status === status) {
                    console.log("éxito");
                    $("#tablaeditortabular").DataTable().ajax.reload();
                    $('#modalPostPutDominio').modal("hide");
                    console.log(data);
                }
            },
            fail: function (data, status) {
                console.log(data, status);
            }
        });
    }

    delete(data) {


    }
    //******************************
}



//señal de iniciaclizació desde JQUERY
$(function () {
    var editor = new editorTabular("http://localhost:5000/api/v1.0/org/Dominio/");
    editor.inicializarEditorTabular();

    var operacion = "";
    //***Pruebas para dominio ***///
    $("#btnCrearDominio").on('click', function () {
        $("#divPostPutDominio").load("http://localhost/Org/Dominio/Crear", function () {
            $("#tituloPostPutDominio").html("Crear Dominio");
            $("#btnPostPut").html("Crear");
            operacion = "post";
            $('#modalPostPutDominio').modal("show");
        });
    });

    $("#btnActualizarDominio").on('click', function () {
        $("#divPostPutDominio").load("http://localhost/Org/Dominio/Editar", function () {
            let row = editor.table.rows(".selected").data()[0];
            let Id = row.Id;
            let nombre = row["Nombre"];



            $("#formPostPutDominio #Id").val(Id);
            $("#formPostPutDominio #Nombre").val(nombre);
            $("#tituloPostPutDominio").html("Actualizar " + nombre);
            $("#btnPostPut").html("Actualizar");
            operacion = "put";
            $('#modalPostPutDominio').modal("show");
        });
    });

    $("#btnEliminar").on('click', function () {
        let rows = editor.table.rows(".selected").data();
        let ids = [];

        if (rows.length >= 1) {
            $("#divPostPutDominio").load("http://localhost/Org/Dominio/Eliminar", function () {

                for (let i = 0; i < rows.length; i++) {
                    let par = i % 2 === 0 ? "" : "2";
                    $("#listaDominios" + par).append("<a class='list-group-item list-group-item-action'>" + rows[i].Nombre + "</a>");
                    ids.push(rows[i].Id );
                }

                $("#formPostPutDominio #idsDelete").val(ids);
                let s = rows.length > 1 ? "s" : "";
                $("#tituloPostPutDominio").html("Eliminar Dominio" + s);
                //$("#btnPostPut").className("danger");
                $("#btnPostPut").html("Eliminar");
                operacion = "delete";
                $('#modalPostPutDominio').modal("show");
            });
        }
    });



    $("#btnPostPut").on('click', function () {
        let div = "#divPostPutDominio";
        let form = $("#formPostPutDominio");
        //let op = $(div + " #formPostPutDominio operacion").val();
        var auxForm = form.serializeArray();
        var formObject = {};
        //$.each(auxForm,
        //    function (i, v) {
        //        formObject[v.name] = v.value;
        //    });



        formObject = {
            "Id": $("#formPostPutDominio #Id").val(),
            "Nombre": $("#formPostPutDominio #Nombre").val(),
            "TipoOrigenId": "Demo",
            "OrigenId": "abc"
        };

        if (operacion === "delete") {
            formObject = $("#formPostPutDominio #idsDelete").val();
            formSplit = formObject.split(',');
            let ids = [];
            for (let i = 0; i < formSplit.length; i++) {
                ids.push(formSplit[i]);
            }
            formObject = { "id": ids };
        }
        console.log(formObject);

        editor.post_put(formObject, operacion);

        //if (operacion === "delete") {
        //    //
        //} else {
        //    editor.post_put(formObject, operacion);
        //}
        
    });
    //******************************



});
