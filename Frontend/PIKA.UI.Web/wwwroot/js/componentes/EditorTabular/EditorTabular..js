//El editor tabular se encarga de realizar las operaciones
//de CRUD para el tipo de entidad exietente en el endpoint de creación
class editorTabular {

    constructor(url) {
        this.url = url;
        this.client = new ClientAPI(url);
        this.tipoEntidad = "";
        this.propiedades = {};
        this.table = "";
    }

    //Punto de entrada para el editor
    async inicializarEditorTabular() {
        this.client.getMetadata().then((respuesta) => {
            if (!respuesta.es_error) {
                console.log(respuesta.datos);
                this.propiedades = respuesta.datos.Propiedades;
                this.tipoEntidad = respuesta.datos.Tipo;
                this.table = this.generaTabla(this.propiedades,
                                this.obtieneUrlEntidad(this.tipoEntidad) + "/datatables");

            } else {
                ///
            }
        });
    }

    obtieneUrlEntidad(tipoEntidad) {
        let url = "";
        let areaTipo = "";
        switch (tipoEntidad) {
            case "UnidadOrganizacional":
                areaTipo = "org/";
                break;
            case "Dominio":
                areaTipo = "org/";
                break;
            case "CuadroClasificacion":
                areaTipo = "gd/";
                break;
            default: console.log("tipoEntidad inválido"); break;
        }
        areaTipo += tipoEntidad;
        url = "http://localhost:5000/api/v1.0/" + areaTipo;
        return url;
    }

    //getTipoEntidad() {
    //    return this.tipoEntidad;
    //}

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
                    columnDefs[columnDefs.length - 1].className = "dt-center";
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

    obtieneNombreProps() {
        let props = [];
        if (this.propiedades.length >= 1) {
            for (let i = 0; i < this.propiedades.length; i++) {
                props.push({ "Nombre": this.propiedades[i].Nombre, "TipoDato": this.propiedades[i].TipoDatoId });
            }
        }
        return props;
    }

    obtienePropsAlternables() {
        let props = [];
        this.propiedades.sort(function (a, b) {
            return a.IndiceOrdenamiento - b.IndiceOrdenamiento;
        });
        if (this.propiedades.length >= 1) {
            for (let i = 0; i < this.propiedades.length; i++) {
                if (this.propiedades[i].AtributoTabla !== null &&
                    this.propiedades[i].AtributoTabla.Alternable === true) {
                    props.push({
                        "Nombre": this.propiedades[i].Nombre,
                        "Visible": this.propiedades[i].AtributoTabla.Visible,
                        "Indice": this.propiedades[i].IndiceOrdenamiento
                        });
                }
            }
        }
        return props;
    }

    // url * tipoEntidad como parámetro
    realizaCrudAPI(data, operacion) {
        let id = data.Id;
        console.log(data);
        if (operacion === "delete") {
            id = "";
            console.log(data);
            for (var i = 0; i < data.id.length; i++) {
                id += "id=" + data.id[i]+"&";
            }
            id = id.slice(0, -1);
        }
        $.ajax({
            url: this.obtieneUrlEntidad(this.tipoEntidad)+ "/" + id,
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
                    $('#modalCrud').modal("hide");
                    console.log(data);
                }
            },
            fail: function (data, status) {
                //manejar excepciones
                console.log("fallo");
            }
        });
    }
}



//señal de iniciaclización desde JQUERY
$(function () {
    var editor = new editorTabular("http://localhost:5000/api/v1.0/org/UnidadOrganizacional/");
    editor.inicializarEditorTabular();
    var operacion = "";

    //*** CRUD ***///
    $("#btnCrearEntidad").on('click', function () {
        $("#divCrud").load("http://localhost/Org/Dominio/Crear", function () {

            operacion = "post";
            mostrarModalCrud(editor.tipoEntidad, "", operacion);

        });
    });

    $("#btnActualizarEntidad").on('click', function () {
        $("#divCrud").load("http://localhost/Org/UnidadOrganizacional/Editar", function () {
            let row = editor.table.rows(".selected").data()[0];
            let nombre = row["Nombre"];
            //$("#formCrud #Id").val(Id);
            //$("#formCrud #Nombre").val(nombre);
            console.log("setBTN");
            setValoresEditar(editor.obtieneNombreProps(), row);

            operacion = "put";
            mostrarModalCrud(editor.tipoEntidad, nombre, operacion);
        });
    });

    $("#btnEliminar").on('click', function () {
        let rows = editor.table.rows(".selected").data();
        let ids = [];

        if (rows.length >= 1) {
            $("#divCrud").load("http://localhost/Org/UnidadOrganizacional/Eliminar", function () {

                for (let i = 0; i < rows.length; i++) {
                    let par = i % 2 === 0 ? "" : "2";
                    $("#listaEntidades" + par).append("<a class='list-group-item list-group-item-action'>" + rows[i].Nombre + "</a>");
                    ids.push(rows[i].Id);
                }

                $("#formCrud #idsDelete").val(ids);
                let s = rows.length > 1 ? "s" : "";

                operacion = "delete";
                mostrarModalCrud(editor.tipoEntidad + s, "", operacion);
            });
        }
    });

    $("#btnCrud").on('click', function () {
        let props = editor.obtieneNombreProps();
        let formObject = {};
        let formSplit = [];
        //let op = $(div + " #formCrud operacion").val();

        if (props.length >= 1) {
            for (let i = 0; i < props.length; i++) {
                if (props[i].TipoDato === "bool") {
                    formObject[props[i].Nombre] = $("#formCrud #" + props[i].Nombre + "").prop("checked");
                } else {
                    formObject[props[i].Nombre] = $("#formCrud #" + props[i].Nombre + "").val();
                }

                // *Prueba unidad org ******
                formObject["DominioId"] = "65cb86a5-6dae-4163-875b-107a35669c49";
                // *Prueba unidad org ******
            }
        } else {
            console.log("No hay propiedades para buscar en el formulario");
        }

        if (operacion === "delete") {
            formObject = $("#formCrud #idsDelete").val();
            formSplit = formObject.split(',');
            let ids = [];
            for (let i = 0; i < formSplit.length; i++) {
                ids.push(formSplit[i]);
            }
            formObject = { "id": ids };
        }

        editor.realizaCrudAPI(formObject, operacion);
    });

    //******************************

    
    // *** Mostrar/ocultar columnas *** 
    var colsVisibles = null;

    $("#btnColumnas").on('click', function () {
        colsVisibles = colsVisibles !== null ? colsVisibles : editor.obtienePropsAlternables();

        $("#listaCols").html("");
        for (let i = 0; i < colsVisibles.length; i++) {
            let active = colsVisibles[i].Visible === true ? "active" : "inactive";
            $("#listaCols").append("<a class='alternable list-group-item list-group-item-action " + active + "' indice=" + colsVisibles[i].Indice + ">"
                + colsVisibles[i].Nombre + "</a>");
        }
        $("#modalCols").modal("show");
    });

    $("#divCols").on('click', ".alternable", function (e) {
        e.preventDefault();
        let col = editor.table.column($(this).attr('indice'));

        colsVisibles[$(this).attr('indice')].Visible = !col.visible();
        col.visible(!col.visible());
        $(this).toggleClass("active");
    });

    $("#btnMostrarCols").on('click', function () {
        $("#modalCols").modal("hide");
    });

    //******************************

    function setValoresEditar(props, valores) {
        for (var i = 0; i < props.length; i++) {
            if (props[i].TipoDato === "bool") {
                if (valores[props[i].Nombre] === true) {
                    $("#formCrud #" + props[i].Nombre).prop("checked", "checked");
                }
            } else {
                $("#formCrud #" + props[i].Nombre).val(valores[props[i].Nombre]);
            }
        }
    }

    function mostrarModalCrud(tipoEntidad, nombreEntidad,operacion) {

        switch (operacion) {
            case "post":
                $("#tituloModalCrud").html("Crear " + tipoEntidad);
                $("#btnCrud").html("Crear");
                $("#btnCrud").removeClass();
                $("#btnCrud").addClass("btn btn-success");
                break;
            case "put":
                $("#tituloModalCrud").html("Actualizar " + nombreEntidad);
                $("#btnCrud").html("Actualizar");
                $("#btnCrud").removeClass();
                $("#btnCrud").addClass("btn btn-primary");
                break;
            case "delete":
                $("#tituloModalCrud").html("Eliminar " + tipoEntidad);
                $("#btnCrud").html("Eliminar");
                $("#btnCrud").removeClass();
                $("#btnCrud").addClass("btn btn-danger");
                break;
            default: console.log("op inválida"); break;
        }
        $('#modalCrud').modal("show");
    }
});
