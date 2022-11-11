using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XafPlayingWithPDF.Module.BusinessObjects
{
    [DefaultClassOptions, NavigationItem("Documents")]
     public class DocumentPackage : XPObject
    { 
        public DocumentPackage(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }



        string subject;
        [RuleRequiredField("You must enter the Subject", DefaultContexts.Save)]
        public string Subject { get { return subject; } set => SetPropertyValue(nameof(Subject), ref subject, value); }

        DateTime startDate;

        [ModelDefault("DisplayFormat", "MM/dd/yyyy HH:mm")]
        [ModelDefault("EditMask", "MM/dd/yyyy HH:mm")]
        public DateTime StartDate
        {
            get { return startDate; }
            set => SetPropertyValue(nameof(StartDate), ref startDate, value);
        }



        private FileData _document;
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        [XafDisplayName("Merged Document")]
        public FileData Document
        {
            get { return _document; }
            set { SetPropertyValue("Document", ref _document, value); }
        }

        #region Asociations
        [XafDisplayName("File(s)")]
        [Association("DocumentPackage-DocumentFile"), DevExpress.Xpo.Aggregated]
        public XPCollection<DocumentFile> DocumentFile
        {
            get { return GetCollection<DocumentFile>(nameof(DocumentFile)); }
        }
        #endregion



    }
}