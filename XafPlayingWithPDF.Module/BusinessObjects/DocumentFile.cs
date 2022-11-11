using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Linq;

namespace XafPlayingWithPDF.Module.BusinessObjects
{
    public class DocumentFile : XPObject
    {
        public DocumentFile(Session session) : base(session)
        {
        }
        public override void AfterConstruction() { base.AfterConstruction(); }


        int order;

        public int Order { get { return order; } set { SetPropertyValue(nameof(Order), ref order, value); } }


        private string description;
        [RuleRequiredField("CourtPackageDescription", DefaultContexts.Save, "You must enter the Description")]
        public string Description
        {
            get { return description; }
            set { SetPropertyValue(nameof(Description), ref description, value); }
        }

        private FileData _document;
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        [XafDisplayName("Document")]
        public FileData Document
        {
            get { return _document; }
            set { SetPropertyValue("Document", ref _document, value); }
        }


        DocumentPackage documentPackage;
        [RuleRequiredField(DefaultContexts.Save)]
        [Association("DocumentPackage-DocumentFile")]
        public DocumentPackage DocumentPackage
        {
            get => documentPackage;
            set => SetPropertyValue(nameof(DocumentPackage), ref documentPackage, value);
        }
    }
}