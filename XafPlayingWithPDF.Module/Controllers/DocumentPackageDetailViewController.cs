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
    public partial class DocumentPackageDetailViewController : ViewController<DetailView>
    {
        public DocumentPackageDetailViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(DocumentPackage);
            TargetViewType = ViewType.DetailView;


            using(SimpleAction createPackage = new SimpleAction(this, "MergeAndRenumber", PredefinedCategory.View))
            {
                createPackage.TargetObjectsCriteria = "DocumentFile.Count > 0";
                createPackage.ImageName = "Action_Export_ToPDF";
                createPackage.Caption = "Merge Documents";
                createPackage.Execute += CreatePackage_Execute;
               
            }
        }

        private void CreatePackage_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            try
            {
                DocumentPackage currentObject = View.CurrentObject as DocumentPackage;
                int currentObjectOid = currentObject.Oid;

                List<string> fileNames = new List<string>();

                if(currentObject != null)
                {
                    if(currentObject.DocumentFile.Count > 0)
                    {
                        foreach(var item in currentObject.DocumentFile.OrderBy(p => p.Order))
                        {
                            FileData currentFile = item.Document;
                            if(currentFile != null && !currentFile.IsEmpty)
                            {
                                using(var stream = new MemoryStream())
                                {
                                    currentFile.SaveToStream(stream);
                                    stream.Position = 0;

                                    FileDataHelper.SaveStreamAsFile(@".\pdf", stream, currentFile.FileName);

                                    string pdfPath = @".\pdf\" + currentFile.FileName;
                                    string fullPath = Path.GetFullPath(pdfPath);

                                    fileNames.Add(fullPath);
                                }
                            }
                        }

                        if(fileNames.Count > 0)
                        {
                            string pdfTargetPath = @".\pdf\" +
                                $"TargetPDF_{DateTime.Now.ToString("MMddyyyyHHmmss")}.pdf";
                            string pdfTargetfullPath = Path.GetFullPath(pdfTargetPath);

                            // --------------------------------------------------
                            // Merge PDF(s)
                            // --------------------------------------------------
                            if(PDFHelper.MergePDFs(fileNames, pdfTargetfullPath))
                            {
                                // ----------------------------------------------
                                // Renumber Documents
                                // ----------------------------------------------
                                string pdfTargetPathR = @".\pdf\" +
                                    $"TargetPDF_{DateTime.Now.ToString("MMddyyyyHHmmss")}R.pdf";
                                string pdfTargetfullPathR = Path.GetFullPath(pdfTargetPathR);

                                if(PDFHelper.EnumeratePDF(pdfTargetfullPath, pdfTargetfullPathR))
                                {
                                    // Store in DB
                                    currentObject.Document = ObjectSpace.CreateObject<FileData>();
                                    currentObject.Document
                                        .LoadFromStream(
                                            $"{currentObject.Subject}.pdf",
                                            new FileStream(pdfTargetfullPathR, FileMode.Open));
                                    currentObject.Save();
                                }
                            }

                            //Refresh Views.
                            ObjectSpace.CommitChanges();
                            View.ObjectSpace.Refresh();
                            ObjectSpace.Refresh();

                            Application.ShowViewStrategy
                                .ShowMessage(
                                    new MessageOptions
                                    {
                                        Duration = 5000,
                                        Message = $"Document Merged and Renumber !!!",
                                        Type = InformationType.Success
                                    });
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
            // Perform various tasks depending on the target View.
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }

        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
