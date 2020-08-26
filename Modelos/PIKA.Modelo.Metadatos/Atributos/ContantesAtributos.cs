using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Atributos
{

    public class ControlUI
    {
        public const string HTML_TOGGLE = "toggle";
        public const string HTML_CHECKBOX = "checkbox";
        public const string HTML_CHECKBOX_MULTI = "checkboxmulti";
        public const string HTML_DATE = "date";
        public const string HTML_TIME = "time";
        public const string HTML_DATETIME = "datetime";
        public const string HTML_FILE = "file";
        public const string HTML_HIDDEN = "hidden";
        public const string HTML_NUMBER = "number";
        public const string HTML_RADIO = "radio";
        public const string HTML_TEXT = "textbox";
        public const string HTML_TEXTAREA = "textarea";
        public const string HTML_SELECT = "select";
        public const string HTML_PASSWORD = "password";
        public const string HTML_PASSWORD_CONFIRM = "passconfirm";
        public const string HTML_AVATAR = "avatar";
        public const string HTML_NONE = "none";
        public const string HTML_LABEL = "label";

        public const string PLATAFORMA_WEB = "web";
    }



    public enum TipoCardinalidad
    {
        UnoVarios = 0, UnoUno = 1
    }

    public enum TipoDespliegueVinculo
    {
       Tabular = 1, Jerarquico =2, EntidadUnica = 3, GrupoCheckbox =10, ListaMultiple =20
    }


    public enum Eventos
    {
        AlCambiar = 1
    }

    public enum Operaciones
    {
        Actualizar = 1, Mostrar=10
    }


    public enum Acciones
    {
        none = 0, add = 1, update = 2, delete = 4, search=8, 
        addupdate=3
    }


}
