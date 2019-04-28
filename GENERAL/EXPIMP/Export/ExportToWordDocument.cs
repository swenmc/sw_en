using BaseClasses;
using DATABASE;
using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using Xceed.Words.NET;

namespace EXPIMP
{
    public static class ExportToWordDocument
    {
        private const string resourcesFolderPath = "./../../Resources/";
        public static void ReportAllDataToWordDoc(Viewport3D viewPort, CModelData modelData, List<string[]> tableParams)
        {
            string fileName = GetReportName();
            // Create a new document.
            using (DocX document = DocX.Create(fileName))
            {
                // The path to a template document,
                string templatePath = resourcesFolderPath + "TemplateReport.docx";
                // Apply a template to the document based on a path.
                document.ApplyTemplate(templatePath);

                DrawModel3DToDoc(document, viewPort);
                DrawProjectInfo(document, GetProjectInfo());
                DrawBasicGeometry(document, modelData);
                DrawMaterial(document, modelData);
                DrawCrossSections(document, modelData);
                DrawComponentList(document, modelData);

                //DrawLogoAndProjectInfoTable(document);
                //DrawLogo(document);
                //DrawModel3D(document, viewPort);
                //DrawBasicGeometry(document, null);
                //DrawLoad(document, null);



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



        private static CProjectInfo GetProjectInfo()
        {
            CProjectInfo pInfo = new CProjectInfo("New self storage", "8 Forest Road, Stoke", "B6351", "Building 1", DateTime.Now);
            return pInfo;
        }

        private static void DrawProjectInfo(DocX document, CProjectInfo pInfo)
        {
            document.ReplaceText("[ProjectName]", pInfo.ProjectName);
            document.ReplaceText("[ProjectSite]", pInfo.Site);
            document.ReplaceText("[ProjectNumber]", pInfo.ProjectNumber);
            document.ReplaceText("[ProjectPart]", pInfo.ProjectPart);
        }
        private static void DrawBasicGeometry(DocX document, CModelData data)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            document.ReplaceText("[GableWidth]", data.GableWidth.ToString(nfi));
            document.ReplaceText("[Length]", data.Length.ToString(nfi));
            document.ReplaceText("[WallHeight]", data.WallHeight.ToString(nfi));
            document.ReplaceText("[RoofPitch_deg]", data.RoofPitch_deg.ToString(nfi));
            document.ReplaceText("[GirtDistance]", data.GirtDistance.ToString(nfi));
            document.ReplaceText("[PurlinDistance]", data.PurlinDistance.ToString(nfi));
            document.ReplaceText("[ColumnDistance]", data.ColumnDistance.ToString(nfi));

            //document.ReplaceText("[RoofPitch_deg]", );
            //document.ReplaceText("[RoofPitch_deg]", );
        }
        private static void DrawMaterial(DocX document, CModelData data)
        {
            var diffMaterials = data.ComponentList.Select(c => c.Material).Distinct();
            Paragraph par = document.Paragraphs.FirstOrDefault(p => p.Text.Contains("[MaterialProperties]"));
            par.RemoveText(0);
            foreach (string material in diffMaterials)
            {
                par = par.InsertParagraphAfterSelf("Material grade: ").Bold().Append(material);

                // Material properties
                List<string> listMaterialPropertyValue = CMaterialManager.LoadMaterialPropertiesStringList(material);

                for (int i = 0; i < data.MaterialDetailsList.Count; i++)
                {
                    data.MaterialDetailsList[i].Value = listMaterialPropertyValue[i];
                }
                data.MaterialDetailsList = new List<CMaterialPropertiesText>(data.MaterialDetailsList);

                par = DrawMaterialTable(document, par, data.MaterialDetailsList);
            }

        }
        // TO Ondrej - Moje uvahy o jednom bazovom rieseni pre tabulky :-)

        // Dalo by sa metodu DrawMaterialTable zobecnit tak, ze List<CMaterialPropertiesText> a List<CSectionPropertiesText>
        // pripadne aj dalsie zobrazovane hodnoty v datagridoch, ktore maju rovnake (podobne) nazvy stlpcov (Load parameters, Member Design, Joint Design) by mali nejakeho spolocneho predka
        // V tom by boli zadefinovane stlpce

        /*
        0 - Text (alebo Name, alebo Description)
        1 - Symbol
        2 - Value
        3 - Unit
        4 - Formula (alebo Equation)
        5 - Note
        */

        // A sprava zobrazovania v Datagridoch GUI a pri exporte do Wordu by bola pre vsetky tieto zoznamy spolocna
        // Zobrazenie pre jednotlive stlpce by sa dalo vypinat a zapinat
        // Dalo by sa pocet riadkov tabulky zmensit tak, ze by sa mohli vlozit napriklad dve alebo tri sekvencie (1-5) vedla seba,
        // takze z 9 riadkov x 5 stlpcov by sme teoreticky mohli urobit 3 riadky x 15 stlpcov (nieco podobne ako je teraz v Member Design Details)
        // Samozrejme by to bolo nastavitelne podla sirky datagridu v GUI alebo sirky stranky A4 bez okrajov
        // Spolocne by sa riesili horne a dolne indexy a jednotky

        private static Paragraph DrawMaterialTable(DocX document, Paragraph p, List<CMaterialPropertiesText> details)
        {
            var t = document.AddTable(1, 4);
            t.Design = TableDesign.TableGrid;
            t.Alignment = Alignment.left;
            t.AutoFit = AutoFit.Window;


            t.Rows[0].Cells[0].Paragraphs[0].InsertText("Text");
            t.Rows[0].Cells[1].Paragraphs[0].InsertText("Symbol");
            t.Rows[0].Cells[2].Paragraphs[0].InsertText("Value");
            t.Rows[0].Cells[3].Paragraphs[0].InsertText("Unit");
            t.Rows[0].Cells[0].Paragraphs[0].Bold();
            t.Rows[0].Cells[1].Paragraphs[0].Bold();
            t.Rows[0].Cells[2].Paragraphs[0].Bold();
            t.Rows[0].Cells[3].Paragraphs[0].Bold();
            t.Rows[0].Cells[0].Width = document.PageWidth * 0.6;
            t.Rows[0].Cells[1].Width = document.PageWidth * 0.13;
            t.Rows[0].Cells[2].Width = document.PageWidth * 0.13;
            t.Rows[0].Cells[3].Width = document.PageWidth * 0.13;

            foreach (CMaterialPropertiesText prop in details)
            {
                if (string.IsNullOrEmpty(prop.Value)) continue;

                Row row = t.InsertRow();
                row.Cells[0].Paragraphs[0].InsertText(prop.Text);
                row.Cells[1].Paragraphs[0].InsertText(prop.Symbol);
                row.Cells[2].Paragraphs[0].InsertText(prop.Value);
                row.Cells[3].Paragraphs[0].InsertText(prop.Unit_NmmMpa);
            }
            p = p.InsertParagraphAfterSelf(p);
            p.RemoveText(0);
            p.InsertTableBeforeSelf(t);
            //p.SpacingAfter(10d);

            return p;
        }
        
        private static void DrawCrossSections(DocX document, CModelData data)
        {
            var diffCrsc = data.ComponentList.Select(c => c.Section).Distinct();
            Paragraph par = document.Paragraphs.FirstOrDefault(p => p.Text.Contains("[CrossSections]"));
            par.RemoveText(0);
            foreach (string crsc in diffCrsc)
            {
                par = par.InsertParagraphAfterSelf("Cross-section name: ").Bold().Append(crsc);

                // Cross-section properties
                List<string> listSectionPropertyValue = CSectionManager.LoadSectionPropertiesStringList(crsc);

                for (int i = 0; i < data.ComponentDetailsList.Count; i++)
                {
                    data.ComponentDetailsList[i].Value = listSectionPropertyValue[i];
                }
                data.ComponentDetailsList = new List<CSectionPropertiesText>(data.ComponentDetailsList);

                par = DrawCrossSectionTable(document, par, crsc, data.ComponentDetailsList);
            }

        }
        private static Paragraph DrawCrossSectionTable(DocX document, Paragraph p, string crsc, List<CSectionPropertiesText> details)
        {
            var t = document.AddTable(1, 5);
            t.Design = TableDesign.TableGrid;
            t.Alignment = Alignment.left;
            t.AutoFit = AutoFit.Contents;

            var picWidth = 200;
            var picHeight = 400;
            var image = document.AddImage($"{resourcesFolderPath}crsc{crsc}.jpg"); // TO Ondrej - co je lepsie JPG alebo PNG format ??? ak  je priehladny tak png a inak je to jedno
            // Set Picture Height and Width.
            var picture = image.CreatePicture(picHeight, picWidth);

            // Insert Picture in paragraph.
            t.Rows[0].Cells[0].Paragraphs[0].AppendPicture(picture);
            t.Rows[0].Cells[1].Paragraphs[0].InsertText("Text");
            t.Rows[0].Cells[2].Paragraphs[0].InsertText("Symbol");
            t.Rows[0].Cells[3].Paragraphs[0].InsertText("Value");
            t.Rows[0].Cells[4].Paragraphs[0].InsertText("Unit");
            t.Rows[0].Cells[1].Paragraphs[0].Bold();
            t.Rows[0].Cells[2].Paragraphs[0].Bold();
            t.Rows[0].Cells[3].Paragraphs[0].Bold();
            t.Rows[0].Cells[4].Paragraphs[0].Bold();

            t.Rows[0].Cells[0].Width = picWidth;
            t.Rows[0].Cells[1].Width = (document.PageWidth - picWidth) * 0.7;
            t.Rows[0].Cells[2].Width = (document.PageWidth - picWidth) * 0.1;
            t.Rows[0].Cells[3].Width = (document.PageWidth - picWidth) * 0.1;
            t.Rows[0].Cells[4].Width = (document.PageWidth - picWidth) * 0.1;
            
            foreach (CSectionPropertiesText prop in details)
            {
                if (string.IsNullOrEmpty(prop.Value)) continue;

                Row row = t.InsertRow();
                row.Cells[1].Paragraphs[0].InsertText(prop.Text);
                row.Cells[2].Paragraphs[0].InsertText(prop.Symbol);
                row.Cells[3].Paragraphs[0].InsertText(prop.Value);
                row.Cells[4].Paragraphs[0].InsertText(prop.Unit_NmmMpa);
            }

            t.MergeCellsInColumn(0, 0, t.Rows.Count-1);

            p = p.InsertParagraphAfterSelf(p);
            p.RemoveText(0);
            p.InsertTableBeforeSelf(t);

            return p;
        }

        private static void DrawComponentList(DocX document, CModelData data)
        {
            Paragraph par = document.Paragraphs.FirstOrDefault(p => p.Text.Contains("[MemberTypes]"));
            par.RemoveText(0);

            var t = document.AddTable(1, 5);
            t.Design = TableDesign.TableGrid;
            t.Alignment = Alignment.left;
            t.AutoFit = AutoFit.Window;


            t.Rows[0].Cells[0].Paragraphs[0].InsertText("Prefix");
            t.Rows[0].Cells[1].Paragraphs[0].InsertText("Color");
            t.Rows[0].Cells[2].Paragraphs[0].InsertText("ComponentName");
            t.Rows[0].Cells[3].Paragraphs[0].InsertText("Section");
            t.Rows[0].Cells[4].Paragraphs[0].InsertText("Material");
            t.Rows[0].Cells[0].Paragraphs[0].Bold();
            t.Rows[0].Cells[1].Paragraphs[0].Bold();
            t.Rows[0].Cells[2].Paragraphs[0].Bold();
            t.Rows[0].Cells[3].Paragraphs[0].Bold();
            t.Rows[0].Cells[4].Paragraphs[0].Bold();
            t.Rows[0].Cells[0].Width = document.PageWidth * 0.1;
            t.Rows[0].Cells[1].Width = document.PageWidth * 0.1;
            t.Rows[0].Cells[2].Width = document.PageWidth * 0.4;
            t.Rows[0].Cells[3].Width = document.PageWidth * 0.2;
            t.Rows[0].Cells[4].Width = document.PageWidth * 0.2;

            foreach (CComponentInfo cInfo in data.ComponentList)
            {
                Row row = t.InsertRow();
                row.Cells[0].Paragraphs[0].InsertText(cInfo.Prefix);

                // Component color
                System.Drawing.Color cellColor = System.Drawing.ColorTranslator.FromHtml(cInfo.Color); // HEX color code
                row.Cells[1].Paragraphs[0].InsertText(cInfo.Color);

                // Exception pokial nebola farba urcena alebo je chyba v kode farby
                try
                {
                    row.Cells[1].Paragraphs[0].Color(cellColor);
                    row.Cells[1].FillColor = cellColor;
                }
                catch(ArgumentException e)
                { }

                row.Cells[2].Paragraphs[0].InsertText(cInfo.ComponentName);
                row.Cells[3].Paragraphs[0].InsertText(cInfo.Section);
                row.Cells[4].Paragraphs[0].InsertText(cInfo.Material);
            }

            par.InsertTableBeforeSelf(t);
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
        }
        private static void DrawModel3DToDoc(DocX document, Viewport3D viewPort)
        {
            Paragraph par = document.Paragraphs.FirstOrDefault(p => p.Text.Contains("[3DModelImage]"));

            ExportHelper.SaveViewPortContentAsImage(viewPort);
            double ratio = viewPort.ActualWidth / viewPort.ActualHeight;

            // Add a simple image from disk.
            var image = document.AddImage("ViewPort.png");
            // Set Picture Height and Width.
            var picture = image.CreatePicture((int)document.PageWidth, (int)(document.PageWidth * ratio));
            // Insert Picture in paragraph.
            par.RemoveText(0);
            par.AppendPicture(picture);
            //par.InsertPageBreakAfterSelf();
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




        //private static void DrawBasicGeometry(DocX document, CModelData data)
        //{            
        //    Paragraph p = document.InsertParagraph("Basic Geometry - Building");
        //    p.StyleName = "Heading1";

        //    p = p.InsertParagraphAfterSelf("Width\t\tB = \t\t10.00 m").Append("2").Script(Script.superscript);
        //    p = p.InsertParagraphAfterSelf("Length\t\tL = \t\t22.85 m").Append("2").Script(Script.superscript);
        //    p = p.InsertParagraphAfterSelf("Height\t\tH").Append("1").Script(Script.subscript).Append(" = \t\t5.00 m").Append("2").Script(Script.superscript);
        //    p = p.InsertParagraphAfterSelf("Height\t\tH").Append("2").Script(Script.subscript).Append(" = \t\t4.48 m").Append("2").Script(Script.superscript);
        //    p = p.InsertParagraphAfterSelf("Pitch\t\tα = \t\t3.0 deg");

        //    p.SpacingAfter(40d);
        //    p = p.InsertParagraphAfterSelf("3D structural model");
        //    p.StyleName = "Heading1";

        //    p = p.InsertParagraphAfterSelf("Theoretical dimensions");
        //    p = p.InsertParagraphAfterSelf("Width\t\tB = \t\t9.41 m");
        //    p = p.InsertParagraphAfterSelf("Length\t\tL = \t\t22.85 m");
        //    p = p.InsertParagraphAfterSelf("Height\t\tH").Append("1").Script(Script.subscript).Append(" = \t\t5.00 m");
        //    p = p.InsertParagraphAfterSelf("Height\t\tH").Append("2").Script(Script.subscript).Append(" = \t\t4.48 m");

        //    p = p.InsertParagraphAfterSelf("Rafter length \t\t9.423 m");
        //    p = p.InsertParagraphAfterSelf("Purlin spacing \t\t1.88 m \t- not used for purlin design");
        //    p = p.InsertParagraphAfterSelf("Girt spacing \t\t2.00 m \t- not used for girt design");
        //}

        //private static void DrawLogoAndProjectInfoTable(DocX document)
        //{
        //    // Add a Table of 5 rows and 2 columns into the document and sets its values.
        //    var t = document.AddTable(1, 3);
        //    t.Design = TableDesign.None;
        //    t.Alignment = Alignment.left;

        //    var image = document.AddImage(ConfigurationManager.AppSettings["logoForPDF"]);
        //    // Set Picture Height and Width.
        //    var picture = image.CreatePicture(150, 300);

        //    t.Rows[0].Cells[0].Paragraphs[0].AppendPicture(picture);
        //    CProjectInfo pInfo = GetProjectInfo();
        //    Paragraph p = t.Rows[0].Cells[1].Paragraphs[0].InsertParagraphAfterSelf("Project Name: ");
        //    p = p.InsertParagraphAfterSelf(pInfo.ProjectName).Bold();

        //    p = p.InsertParagraphAfterSelf("Site: ");
        //    p = p.InsertParagraphAfterSelf(pInfo.Site).Bold();
        //    p.SpacingAfter(20d);

        //    p = p.InsertParagraphAfterSelf("Project Number: ");
        //    p = p.InsertParagraphAfterSelf(pInfo.ProjectNumber).Bold();

        //    p = t.Rows[0].Cells[2].Paragraphs[0].InsertParagraphAfterSelf("Project Part: ");
        //    p = p.InsertParagraphAfterSelf(pInfo.ProjectPart).Bold();
        //    p.SpacingAfter(50d);

        //    p = p.InsertParagraphAfterSelf("Date: ");
        //    p = p.InsertParagraphAfterSelf(pInfo.Date.ToShortDateString()).Bold();


        //    document.InsertTable(t);
        //}

        //private static void DrawLogo(DocX document)
        //{
        //    // Add a simple image from disk.
        //    var image = document.AddImage(ConfigurationManager.AppSettings["logoForPDF"]);
        //    // Set Picture Height and Width.
        //    var picture = image.CreatePicture(300, 150);

        //    // Insert Picture in paragraph.
        //    var p = document.InsertParagraph();
        //    p.AppendPicture(picture);
        //}

    }
}

