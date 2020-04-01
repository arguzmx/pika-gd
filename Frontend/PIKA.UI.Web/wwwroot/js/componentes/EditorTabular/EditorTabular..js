//El editor tabular se encarga de realizar las operaciones
//de CRUD para el tipo de entidad exietente en el endpoint de creación
class editorTabular {

    constructor(url) {
        this.url = url
        this.client = new ClientAPI(url);
    }

    //Punto de entrada para el editor
    inicializarEditorTabular() {
        this.client.getMetadata().then((respuesta) => {
            if (!respuesta.es_error) {
                console.log(respuesta.datos);

                this.generaTabla(respuesta.datos.Propiedades, 
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
        var t = $('#tablaeditortabular').DataTable({
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
    }
}



//señal de iniciaclizació desde JQUERY
$(function () {
    let editor = new editorTabular("http://localhost:5000/api/v1.0/org/Dominio/");
    editor.inicializarEditorTabular();
});
