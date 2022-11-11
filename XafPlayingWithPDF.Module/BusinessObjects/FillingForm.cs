using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Linq;

namespace XafPlayingWithPDF.Module.BusinessObjects
{
    [DefaultClassOptions, NavigationItem("Documents")]

    public class FillingForm : XPObject
    {
        public FillingForm(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();

        }


        string fieldValue;
        string fieldName;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string FieldName
        {
            get => fieldName;
            set => SetPropertyValue(nameof(FieldName), ref fieldName, value);
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string FieldValue
        {
            get => fieldValue;
            set => SetPropertyValue(nameof(FieldValue), ref fieldValue, value);
        }


    }
}