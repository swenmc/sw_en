using BaseClasses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Controls;
using Xceed.Words.NET;

namespace EXPIMP
{
    public static class ExportToWordDocument
    {
        public static void ReportAllDataToWordDoc(Viewport3D viewPort, CModelData modelData, List<string[]> tableParams)
        {
            string fileName = GetReportName();
            // Create a new document.
            using (DocX document = DocX.Create(fileName))
            {
                // The path to a template document,
                //string templatePath = Server.MapPath("~/res/") + @"Template.docx";
                // Apply a template to the document based on a path.
                //document.ApplyTemplate(templatePath);

                DrawLogoAndProjectInfoTable(document);
                //DrawLogo(document);
                //DrawProjectInfo(document, GetProjectInfo());
                DrawModel3D(document, viewPort);
                DrawBasicGeometry(document, null);
                DrawLoad(document, null);



                //CreateChapterWithBuletedList(document);
                //CreateTOC(document);

                // Save this document to disk.
                document.Save();

            }
            Process.Start(fileName);
        }

        private static string GetReportName()
        {
            int count = 0;
            string fileName = null;
            bool nameOK = false;
            while (!nameOK)
            {
                fileName = $"Report_{++count}.docx";

                if (!System.IO.File.Exists(fileName)) nameOK = true;
            }
            return fileName;
        }

        private static void DrawLogoAndProjectInfoTable(DocX document)
        {
            // Add a Table of 5 rows and 2 columns into the document and sets its values.
            var t = document.AddTable(1, 3);
            t.Design = TableDesign.None;
            t.Alignment = Alignment.left;

            var image = document.AddImage(ConfigurationManager.AppSettings["logoForPDF"]);
            // Set Picture Height and Width.
            var picture = image.CreatePicture(150, 300);

            t.Rows[0].Cells[0].Paragraphs[0].AppendPicture(picture);
            CProjectInfo pInfo = GetProjectInfo();
            Paragraph p = t.Rows[0].Cells[1].Paragraphs[0].InsertParagraphAfterSelf("Project Name: ");
            p = p.InsertParagraphAfterSelf(pInfo.ProjectName).Bold();

            p = p.InsertParagraphAfterSelf("Site: ");
            p = p.InsertParagraphAfterSelf(pInfo.Site).Bold();
            p.SpacingAfter(20d);

            p = p.InsertParagraphAfterSelf("Project Number: ");
            p = p.InsertParagraphAfterSelf(pInfo.ProjectNumber).Bold();

            p = t.Rows[0].Cells[2].Paragraphs[0].InsertParagraphAfterSelf("Project Part: ");
            p = p.InsertParagraphAfterSelf(pInfo.ProjectPart).Bold();
            p.SpacingAfter(50d);

            p = p.InsertParagraphAfterSelf("Date: ");
            p = p.InsertParagraphAfterSelf(pInfo.Date.ToShortDateString()).Bold();

            
            document.InsertTable(t);
        }

        private static void DrawLogo(DocX document)
        {
            // Add a simple image from disk.
            var image = document.AddImage(ConfigurationManager.AppSettings["logoForPDF"]);
            // Set Picture Height and Width.
            var picture = image.CreatePicture(300, 150);

            // Insert Picture in paragraph.
            var p = document.InsertParagraph();
            p.AppendPicture(picture);
        }

        private static CProjectInfo GetProjectInfo()
        {
            CProjectInfo pInfo = new CProjectInfo("New self storage", "8 Forest Road, Stoke", "B6351", "Building 1", DateTime.Now);
            return pInfo;
        }

        private static void DrawProjectInfo(Paragraph par, CProjectInfo pInfo)
        {
            Paragraph p = par.InsertParagraphAfterSelf("Project Name: ");
            p = p.InsertParagraphAfterSelf(pInfo.ProjectName);

            p = p.InsertParagraphAfterSelf("Site: ");
            p = p.InsertParagraphAfterSelf(pInfo.Site);

            p = p.InsertParagraphAfterSelf("Project Number: ");
            p = p.InsertParagraphAfterSelf(pInfo.ProjectNumber);

            p = p.InsertParagraphAfterSelf("Project Part: ");
            p = p.InsertParagraphAfterSelf(pInfo.ProjectPart);

            p = p.InsertParagraphAfterSelf("Date: ");
            p = p.InsertParagraphAfterSelf(pInfo.Date.ToShortDateString());

        }

        private static void DrawBasicGeometry(DocX document, CModelData data)
        {            
            Paragraph p = document.InsertParagraph("Basic Geometry - Building");
            p.StyleName = "Heading1";

            p = p.InsertParagraphAfterSelf("Width\t\tB = \t\t10.00 m").Append("2").Script(Script.superscript);
            p = p.InsertParagraphAfterSelf("Length\t\tL = \t\t22.85 m").Append("2").Script(Script.superscript);
            p = p.InsertParagraphAfterSelf("Height\t\tH").Append("1").Script(Script.subscript).Append(" = \t\t5.00 m").Append("2").Script(Script.superscript);
            p = p.InsertParagraphAfterSelf("Height\t\tH").Append("2").Script(Script.subscript).Append(" = \t\t4.48 m").Append("2").Script(Script.superscript);
            p = p.InsertParagraphAfterSelf("Pitch\t\tα = \t\t3.0 deg");

            p.SpacingAfter(40d);
            p = p.InsertParagraphAfterSelf("3D structural model");
            p.StyleName = "Heading1";

            p = p.InsertParagraphAfterSelf("Theoretical dimensions");
            p = p.InsertParagraphAfterSelf("Width\t\tB = \t\t9.41 m");
            p = p.InsertParagraphAfterSelf("Length\t\tL = \t\t22.85 m");
            p = p.InsertParagraphAfterSelf("Height\t\tH").Append("1").Script(Script.subscript).Append(" = \t\t5.00 m");
            p = p.InsertParagraphAfterSelf("Height\t\tH").Append("2").Script(Script.subscript).Append(" = \t\t4.48 m");

            p = p.InsertParagraphAfterSelf("Rafter length \t\t9.423 m");
            p = p.InsertParagraphAfterSelf("Purlin spacing \t\t1.88 m \t- not used for purlin design");
            p = p.InsertParagraphAfterSelf("Girt spacing \t\t2.00 m \t- not used for girt design");
        }
        private static void DrawLoad(DocX document, CModelData data)
        {
            Paragraph p = document.InsertParagraph("Load");
            p.StyleName = "Heading1";

            p = p.InsertParagraphAfterSelf("Basic parameters");
            p.StyleName = "Heading2";
            p = p.InsertParagraphAfterSelf("Location:");
            p = p.InsertParagraphAfterSelf("Design Life:");
            p = p.InsertParagraphAfterSelf("Importance level:");
            p = p.InsertParagraphAfterSelf("Annual probability of ecxeedance SLS:");


            p = p.InsertParagraphAfterSelf("Dead load (self - weight of frame and cladding)");
            p.StyleName = "Heading2";
            p = p.InsertParagraphAfterSelf("Roof:\t\tPurlindek 0.4 mm \t\tg = 0.05623 kN/m").Append("2").Script(Script.superscript).
                Append("\t5.623 kg/m").Append("2").Script(Script.superscript);
            p = p.InsertParagraphAfterSelf("Wall - gridline \"2\":");

            p = p.InsertParagraphAfterSelf("Service load - long-term load (considered as dead load)");
            p.StyleName = "Heading2";
            p = p.InsertParagraphAfterSelf("Service load - roof:");
            p = p.InsertParagraphAfterSelf("Design Life:");
            

        }

        private static void DrawModel3D(DocX document, Viewport3D viewPort)
        {
            document.InsertParagraph("Structural model in 3D environment: ");

            ExportHelper.SaveViewPortContentAsImage(viewPort);

            // Add a simple image from disk.
            var image = document.AddImage("ViewPort.png");

            // Set Picture Height and Width.
            var picture = image.CreatePicture((int)document.PageWidth, (int)document.PageWidth);
            // Insert Picture in paragraph.
            var p = document.InsertParagraph();
            p.AppendPicture(picture);
            p.InsertPageBreakAfterSelf();


            //XImage image = XImage.FromFile("ViewPort.png");
            //double scaleFactor = gfx.PageSize.Width / image.PointWidth;
            //double scaledImageWidth = gfx.PageSize.Width;
            //double scaledImageHeight = image.PointHeight * scaleFactor;

            //gfx.DrawImage(image, 0, 300, scaledImageWidth, scaledImageHeight);

            //gfx.DrawImage(image, image.Size.Width, image.Size.Height);
        }

        private static void CreateTOC(DocX document)
        {
            document.InsertTableOfContents("Programatically generated TOC", TableOfContentsSwitches.H);
        }

        private static void CreateChapterWithBuletedList(DocX document)
        {
            // Add a paragraph at the end of the template.
            var p1 = document.InsertParagraph("TECHNICAL SOLUTION");
            p1.StyleName = "Heading1";

            var p2 = document.InsertParagraph("Picture");
            p2.StyleName = "No spacing";

            var p3 = document.InsertParagraph("DESCRIPTION OF FUNCTIONALITIES");
            p3.StyleName = "Heading2";

            var p4 = document.InsertParagraph("text of description of functionalities");
            p4.StyleName = "Normal";

            var p5 = document.InsertParagraph("PARTS INCLUDED IN THE DELIVERY");
            p5.StyleName = "Heading2";

            var p6 = document.InsertParagraph("text for PARTS INCLUDED IN THE DELIVERY");
            p6.StyleName = "Normal";

            var p7 = document.InsertParagraph("PARTS EXCLUDED FROM THE DELIVERY");
            p7.StyleName = "Heading2";

            var bulletedList = document.AddList("Mast / pole", 0, ListItemType.Bulleted);
            document.AddListItem(bulletedList, "Batteries");
            document.AddListItem(bulletedList, "Aerials and coaxial cables");
            // Add an item (level 0)
            document.AddListItem(bulletedList, "Radio - stations");
            document.AddListItem(bulletedList, "Radio station 1", 1);
            document.AddListItem(bulletedList, "Radio station 2", 1);
            document.AddListItem(bulletedList, "Push buttons");
            document.AddListItem(bulletedList, "Olflex cables");
            document.AddListItem(bulletedList, "CAN cables");
            document.AddListItem(bulletedList, "Solar panels and accessories");

            document.InsertList(bulletedList);
        }

        //Indentations
        //// Add the first paragraph.
        //var p = document.InsertParagraph("This is the first paragraph. It doesn't contain any indentation.");
        //p.SpacingAfter(30);
        //        // Add the second paragraph.
        //        var p2 = document.InsertParagraph("This is the second paragraph. It contains an indentation on the first line.");
        //// Indent only the first line of the Paragraph.
        //p2.IndentationFirstLine = -1.0f;
        //        p2.SpacingAfter(30);
        //        // Add the third paragraph.
        //        var p3 = document.InsertParagraph("This is the third paragraph. It contains an indentation on all the lines except the first one.");
        //// Indent all the lines of the Paragraph, except the first.
        //p3.IndentationHanging = 1.0f;

    }
}

