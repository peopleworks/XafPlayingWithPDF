using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Linq;

namespace XafPlayingWithPDF.Module.BusinessObjects
{
    [DefaultClassOptions, NavigationItem("Documents")]
    [XafDisplayName("PDF Form")]
    public class PDFForm : XPObject
    {
        public PDFForm(Session session) : base(session)
        {
        }
        public override void AfterConstruction() { base.AfterConstruction(); }


        FileData form;
        string description;
        string formName;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string FormName { get => formName; set => SetPropertyValue(nameof(FormName), ref formName, value); }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Description
        {
            get => description;
            set => SetPropertyValue(nameof(Description), ref description, value);
        }

        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        [FileTypeFilter("AllFiles", 2, "*.pdf")]
        [RuleRequiredField(DefaultContexts.Save)]
        public FileData Form { get => form; set => SetPropertyValue(nameof(Form), ref form, value); }



    }
}