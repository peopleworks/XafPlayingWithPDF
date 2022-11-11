using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using System;
using System.Linq;
using XafPlayingWithPDF.Module.BusinessObjects;
using XafPlayingWithPDF.Module.Helper;
using FileDataHelper = XafPlayingWithPDF.Module.Helper.FileDataHelper;

namespace XafPlayingWithPDF.Module.Controllers
{
    public partial class FillingFormListViewController : ViewController<ListView>
    {
        SingleChoiceAction formPDFilterAction;
        FileData currentFile;

        public FillingFormListViewController()
        {
            InitializeComponent();

            TargetObjectType = typeof(FillingForm);
            TargetViewType = ViewType.ListView;


            using(SimpleAction fillaDocument = new SimpleAction(this, "FillaDocument", PredefinedCategory.View))
            {
                fillaDocument.ImageName = "Action_Export_ToPDF";
                fillaDocument.Caption = "Fill Form";
                fillaDocument.Execute += FillaDocument_Execute;
            }


            // USCIS Form to Filler
            formPDFilterAction = new SingleChoiceAction(this, "FormFilter", PredefinedCategory.Filters)
            {
                Caption = "Select Form To Fill",
                ToolTip = "Select Form To Fill"
            };
            formPDFilterAction.CustomizeControl += FormPDFilterAction_CustomizeControl;
            formPDFilterAction.Execute += FormPDFilterAction_Execute;
        }

        private void FormPDFilterAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            PDFForm uSCISForm = ObjectSpace.GetObjects<PDFForm>()
                .Where(u => u.FormName == e.SelectedChoiceActionItem.Caption)
                .FirstOrDefault();

            if(uSCISForm != null)
            {
                currentFile = uSCISForm.Form;
            }
        }

        private void FormPDFilterAction_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
        }

        private void FillaDocument_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            try
            {
                List<KeyValuePair<string, string>> fieldsAndValues = new List<KeyValuePair<string, string>>();


                foreach(var field in ObjectSpace.GetObjects<FillingForm>()
                    .Where(f => !string.IsNullOrEmpty(f.FieldValue))
                    .OrderBy(f => f.FieldName))
                {
                    fieldsAndValues.Add(new KeyValuePair<string, string>(field.FieldName, field.FieldValue));
                }

                if(fieldsAndValues.Count > 0)
                {                    
                    if(currentFile != null && !currentFile.IsEmpty)
                    {
                        using(var stream = new MemoryStream())
                        {
                            currentFile.SaveToStream(stream);
                            stream.Position = 0;

                            Helper.FileDataHelper.SaveStreamAsFile(@".\pdf", stream, $"Source_{currentFile.FileName}");

                            // Source
                            string rptPathSource = @".\pdf\Source_" + currentFile.FileName;
                            string fullPathSource = Path.GetFullPath(rptPathSource);

                            // Target
                            string rptPathTarget = @".\pdf\Target_" + currentFile.FileName;
                            string fullPathTarget = Path.GetFullPath(rptPathTarget);


                            if(PDFHelper.FillPDFForm(fullPathSource, fullPathTarget, fieldsAndValues))
                            {
                                Application.ShowViewStrategy
                                    .ShowMessage(
                                        new MessageOptions
                                        {
                                            Duration = 5000,
                                            Message = "Form Filled !! ",
                                            Type = InformationType.Success
                                        });

                                FileDataHelper.OpenWithDefaultProgram(fullPathTarget);
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            try
            {
                formPDFilterAction.Items.Clear();

                formPDFilterAction.Items.Add(new ChoiceActionItem("Select Form", "Select Form"));
                foreach(PDFForm pDFForm in ObjectSpace.GetObjects<PDFForm>().OrderBy(f => f.FormName))
                {
                    formPDFilterAction.Items.Add(new ChoiceActionItem(pDFForm.FormName, pDFForm.Oid));
                }

                formPDFilterAction.SelectedIndex = 0;
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override void OnViewControlsCreated() { base.OnViewControlsCreated(); }
        protected override void OnDeactivated() { base.OnDeactivated(); }
    }
}
