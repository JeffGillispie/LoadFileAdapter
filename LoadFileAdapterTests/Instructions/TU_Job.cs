﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoadFileAdapter;
using LoadFileAdapter.Parsers;
using LoadFileAdapter.Instructions;
using LoadFileAdapter.Builders;
using LoadFileAdapter.Transformers;
using LoadFileAdapter.Exporters;
using Moq;

namespace LoadFileAdapterTests
{
    [TestClass]
    public class TU_Job
    {
        [TestMethod]
        public void Instructions_Job_DatImport()
        {
            // arrange
            FileInfo infile = new FileInfo("x:\\test\\testfile.dat");
            Encoding encoding = Encoding.GetEncoding(1252);
            Delimiters delims = Delimiters.COMMA_DELIMITED;
            List<RepresentativeBuilder> linkedFiles = new List<RepresentativeBuilder>();
            linkedFiles.Add(new RepresentativeBuilder("NativeLink", Representative.FileType.Native));
            DatImport import = new DatImport(infile, encoding, delims, "DOCID");
            import.HasHeader = true;
            import.ParentColumnName = "BEGATT";
            import.ChildColumnName = "ATTIDS";
            import.ChildColumnDelimiter = ";";
            import.LinkedFiles = linkedFiles.Select(f => new RepresentativeInfo(f)).ToArray();                                
            Job job = new Job(new Import[] { import }, null, null);
            
            // act
            string xml = job.ToXml();
            Job testJob = Job.Deserialize(xml);

            // assert
            Assert.AreEqual(job.Imports[0].Encoding, testJob.Imports[0].Encoding);
            Assert.AreEqual(job.Imports[0].File.FullName, testJob.Imports[0].File.FullName);
            Assert.AreEqual(
                ((DatImport)job.Imports[0]).KeyColumnName, 
                ((DatImport)testJob.Imports[0]).KeyColumnName
                );
            Assert.AreEqual(
                ((DatImport)job.Imports[0]).ParentColumnName,
                ((DatImport)testJob.Imports[0]).ParentColumnName
                );
            Assert.AreEqual(
                ((DatImport)job.Imports[0]).ChildColumnName,
                ((DatImport)testJob.Imports[0]).ChildColumnName
                );
            Assert.AreEqual(
                ((DatImport)job.Imports[0]).ChildColumnDelimiter,
                ((DatImport)testJob.Imports[0]).ChildColumnDelimiter
                );
            Assert.AreEqual(
                ((DatImport)job.Imports[0]).HasHeader,
                ((DatImport)testJob.Imports[0]).HasHeader
                );
            
            for (int i = 0; i < ((DatImport)job.Imports[0]).LinkedFiles.Length; i++)
            {
                Assert.AreEqual(
                    ((DatImport)job.Imports[0]).LinkedFiles[i].ColumnName,
                    ((DatImport)testJob.Imports[0]).LinkedFiles[i].ColumnName
                    );
                Assert.AreEqual(
                    ((DatImport)job.Imports[0]).LinkedFiles[i].FileType,
                    ((DatImport)testJob.Imports[0]).LinkedFiles[i].FileType
                    );
            }
        }

        [TestMethod]
        public void Instructions_Job_ImgImport()
        {
            // arrange
            FileInfo infile = new FileInfo("x:\\test\\testfile.lfp");
            Encoding encoding = Encoding.GetEncoding(1252);            
            TextBuilder textSetting = new TextBuilder(
                TextBuilder.TextLevel.Page,
                TextBuilder.TextLocation.AlternateLocation,
                new Regex("IMAGES"),
                "TEXT"
                );            
            Import import = new LfpImport(infile, encoding, textSetting, true);
            Job job = new Job(new Import[] { import }, null, null);

            // act
            string xml = job.ToXml();
            Job testJob = Job.Deserialize(xml);

            // assert
            Assert.AreEqual(job.Imports[0].Encoding, testJob.Imports[0].Encoding);
            Assert.AreEqual(job.Imports[0].File.FullName, testJob.Imports[0].File.FullName);
            Assert.AreEqual(
                ((ImgImport)job.Imports[0]).TextBuilder.FileLevel,
                ((ImgImport)testJob.Imports[0]).TextBuilder.FileLevel
                );
            Assert.AreEqual(
                ((ImgImport)job.Imports[0]).TextBuilder.FileLocation,
                ((ImgImport)testJob.Imports[0]).TextBuilder.FileLocation
                );
            Assert.AreEqual(
                ((ImgImport)job.Imports[0]).TextBuilder.PathFind.ToString(),
                ((ImgImport)testJob.Imports[0]).TextBuilder.PathFind.ToString()
                );
            Assert.AreEqual(
                ((ImgImport)job.Imports[0]).TextBuilder.PathReplace,
                ((ImgImport)testJob.Imports[0]).TextBuilder.PathReplace
                );            
        }

        [TestMethod]
        public void Instructions_Job_Edits()
        {
            // arrange
            List<Transformation> edits = new List<Transformation>();
            edits.Add(MetaDataTransformation.Builder
                .Start("field name", new Regex("find text"), "replace text", "filter field", new Regex("filter pattern"))
                .SetAltDestinationField("alt destination")
                .SetPrependField("preprend field")
                .SetAppendField("append field")
                .SetJoinDelimiter("join delim")
                .SetPrependDir(new DirectoryInfo("X:\\path"))
                .Build());
            edits.Add(RepresentativeTransformation.Builder
                .Start(Representative.FileType.Image, new Regex("find"), "replace")
                .SetFilterField("filter field")
                .SetFilterText(new Regex("filter text"))
                .Build());                
            Job job = new Job(null, null, edits.ToArray());

            // act
            string xml = job.ToXml();
            Job testJob = Job.Deserialize(xml);

            // assert
            for (int i = 0; i < job.Edits.Length; i++)
            {
                Transformation editA = job.Edits[i].ToTransformation();
                
                if (editA.GetType().Equals(typeof(MetaDataTransformation)))
                {
                    MetaDataTransformation a = (MetaDataTransformation)editA;
                    MetaDataTransformation b = (MetaDataTransformation)testJob.Edits[i].ToTransformation();
                    Assert.AreEqual(a.AlternateDestinationField, b.AlternateDestinationField);
                    Assert.AreEqual(a.AppendField, b.AppendField);
                    Assert.AreEqual(a.FieldName, b.FieldName);
                    Assert.AreEqual(a.FilterField, b.FilterField);
                    Assert.AreEqual(a.FilterText.ToString(), b.FilterText.ToString());
                    Assert.AreEqual(a.FindText.ToString(), b.FindText.ToString());
                    Assert.AreEqual(a.JoinDelimiter, b.JoinDelimiter);
                    Assert.AreEqual(a.PrependDirectory.FullName, b.PrependDirectory.FullName);
                    Assert.AreEqual(a.PrependField, b.PrependField);
                    Assert.AreEqual(a.ReplaceText, b.ReplaceText);                    
                }
                else
                {
                    RepresentativeTransformation a = (RepresentativeTransformation)editA;
                    RepresentativeTransformation b = (RepresentativeTransformation)testJob.Edits[i].ToTransformation();
                    Assert.AreEqual(a.FilterField, b.FilterField);
                    Assert.AreEqual(a.FilterText.ToString(), b.FilterText.ToString());
                    Assert.AreEqual(a.FindText.ToString(), b.FindText.ToString());
                    Assert.AreEqual(a.NewType, b.NewType);
                    Assert.AreEqual(a.ReplaceText, b.ReplaceText);
                    Assert.AreEqual(a.TargetType, b.TargetType);
                }
            }
        }

        [TestMethod]
        public void Instructions_Job_DatExport()
        {
            // arrange
            FileInfo file = new FileInfo("x:\\test\\testfile.dat");
            Encoding encoding = Encoding.GetEncoding(1252);
            Delimiters delims = Delimiters.CONCORDANCE;
            string[] fields = new string[] { "DOCID", "BEGATT", "VOLUME" };
            Export instructions = new DatExport(file, encoding, delims, fields);
            Job job = new Job(null, new Export[] { instructions }, null);

            // act
            string xml = job.ToXml();
            Job testJob = Job.Deserialize(xml);

            // assert
            Assert.AreEqual(job.Exports[0].File.FullName, testJob.Exports[0].File.FullName);
            Assert.AreEqual(job.Exports[0].Encoding, testJob.Exports[0].Encoding);
            Assert.AreEqual(
                String.Join(",", ((DatExport)job.Exports[0]).ExportFields), 
                String.Join(",", ((DatExport)testJob.Exports[0]).ExportFields));
            Assert.AreEqual(
                ((DatExport)job.Exports[0]).Delimiters.FieldSeparator, 
                ((DatExport)testJob.Exports[0]).Delimiters.FieldSeparator);
            Assert.AreEqual(
                ((DatExport)job.Exports[0]).Delimiters.TextQualifier,
                ((DatExport)testJob.Exports[0]).Delimiters.TextQualifier);
            Assert.AreEqual(
                ((DatExport)job.Exports[0]).Delimiters.EscapeCharacter,
                ((DatExport)testJob.Exports[0]).Delimiters.EscapeCharacter);
            Assert.AreEqual(
                ((DatExport)job.Exports[0]).Delimiters.NewRecord,
                ((DatExport)testJob.Exports[0]).Delimiters.NewRecord);
            Assert.AreEqual(
                ((DatExport)job.Exports[0]).Delimiters.FlattenedNewLine,
                ((DatExport)testJob.Exports[0]).Delimiters.FlattenedNewLine);
        }

        [TestMethod]
        public void Instructions_Job_ImgExport()
        {
            // arrange
            FileInfo file = new FileInfo("x:\\test\\testfile.opt");
            Encoding encoding = Encoding.GetEncoding(1252);
            Export instructions = new OptExport(file, encoding, "TEST001");            
            Job job = new Job(null, new Export[] { instructions }, null);

            // act
            string xml = job.ToXml();
            Job testJob = Job.Deserialize(xml);

            // assert
            Assert.AreEqual(job.Exports[0].File.FullName, testJob.Exports[0].File.FullName);
            Assert.AreEqual(job.Exports[0].Encoding, testJob.Exports[0].Encoding);
            Assert.AreEqual(
                ((ImgExport)job.Exports[0]).VolumeName,
                ((ImgExport)testJob.Exports[0]).VolumeName);
        }

        [TestMethod]
        public void Instructions_Job_All()
        {
            // arrange
            List<Import> imports = new List<Import>();            
            DatImport import = new DatImport(
                new FileInfo("x:\\test\\test.csv"),
                Encoding.Unicode,
                Delimiters.COMMA_QUOTE,
                "DOCID");
            import.HasHeader = true;
            import.ParentColumnName = "PARENT";
            import.ChildColumnName = null;
            import.ChildColumnDelimiter = ";";
            import.LinkedFiles = new RepresentativeInfo[] {
                new RepresentativeInfo(new RepresentativeBuilder("NATIVE", Representative.FileType.Native))
            };
            import.FolderPrependFields = new string[] { "ITEMPATH" };
            import.FolderPrependLinks = new Representative.FileType[] { Representative.FileType.Native };
            imports.Add(import);                
            imports.Add(new LfpImport(
                new FileInfo("x:\\test\\test.lfp"),
                Encoding.GetEncoding(1252),
                new TextBuilder(
                    TextBuilder.TextLevel.Page, 
                    TextBuilder.TextLocation.AlternateLocation, 
                    new Regex(@"\\IMAGES", RegexOptions.IgnoreCase), 
                    "\\TEXT"), 
                true
                ));
            List<Export> exports = new List<Export>();
            exports.Add(new DatExport(
                new FileInfo("x:\\test\\test.dat"), 
                Encoding.Unicode, 
                Delimiters.CONCORDANCE, 
                new string[] { "one", "two", "buckle", "my", "shoe" }));
            exports.Add(new OptExport(
                new FileInfo("x:\\test\\test.opt"), 
                Encoding.Unicode, 
                "TEST001"));
            exports.Add(new XlsExport(
                new FileInfo("x:\\test\\test.xlsx"), 
                new string[] { "three", "four", "shut", "the", "door" }, 
                new HyperLinkInfo[] { new HyperLinkInfo(Representative.FileType.Native, "displayText", 2) }));
            var xref = new XrefExport();
            xref.File = new FileInfo("x:\\test\\CrossRef.txt");
            xref.CustomerDataField = "CustomerData";
            xref.NamedFolderField = "NamedFolder";
            xref.NamedFileField = "NamedFile";
            xref.CodePage = 1251;
            Trigger boxTrigger = new Trigger();
            Trigger csTrigger = new Trigger();
            Trigger gsTrigger = new Trigger();
            boxTrigger.Type = Switch.SwitchType.FieldValueChange;
            boxTrigger.FieldChangeOption = Switch.ValueChangeOption.StripFileName;
            boxTrigger.FieldName = "FilePath";
            csTrigger.Type = Switch.SwitchType.Regex;
            csTrigger.RegexPattern = "\\d";
            csTrigger.FieldName = "Group";
            gsTrigger.Type = Switch.SwitchType.FieldValueChange;
            gsTrigger.FieldChangeOption = Switch.ValueChangeOption.UseStartingSegments;
            gsTrigger.SegmentCount = 2;
            gsTrigger.SegmentDelimiter = "\\";            
            xref.BoxTrigger = boxTrigger;
            xref.CodeStartTrigger = csTrigger;
            xref.GroupStartTrigger = gsTrigger;
            var ss = new SlipsheetsInfo();
            ss.BindSlipsheets = false;
            ss.FolderName = "SlipSheets";
            ss.FontSize = 12;
            ss.FontStyle = System.Drawing.FontStyle.Bold;
            ss.FontFamilyName = "Arial";
            ss.HorizontalPlacement = SlipSheet.HorizontalPlacementOption.Center;
            ss.VerticalPlacement = SlipSheet.VerticalPlacementOption.Center;
            ss.Resolution = 300;
            ss.UseFieldLabels = true;
            Trigger ssTrigger = new Trigger();
            ssTrigger.Type = Switch.SwitchType.Family;
            ss.Trigger = ssTrigger;
            List<SlipsheetField> ssFields = new List<SlipsheetField>();
            ssFields.Add(new SlipsheetField("DOCID", "BegNo"));
            ssFields.Add(new SlipsheetField("DOCTYPE", "FileType"));
            ss.Fields = ssFields.ToArray();
            xref.Slipsheets = ss;
            exports.Add(xref);
            List<Transformation> edits = new List<Transformation>();
            edits.Add(MetaDataTransformation.Builder
                .Start("field name", new Regex("find text"), "replace text", "filter field", new Regex("filter pattern"))
                .SetAltDestinationField("alt destination")
                .SetPrependField("preprend field")
                .SetAppendField("append field")
                .SetJoinDelimiter("join delim")
                .SetPrependDir(new DirectoryInfo("X:\\path"))
                .Build());                
            edits.Add(RepresentativeTransformation.Builder
                .Start(Representative.FileType.Text, new Regex("find"), "replace")
                .SetFilterField("filter field")
                .SetFilterText(new Regex("filter text"))
                .Build());            
            var dateEdit = new DateFormatEdit();
            dateEdit.FieldName = "DateCreated";
            dateEdit.InputFormat = "dd/MM/yyyy";
            TimeZoneInfo outTZ = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            TimeZoneInfo inTZ = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            dateEdit.InputTimeZone = inTZ;
            dateEdit.OutputTimeZone = outTZ;
            dateEdit.OutputFormat = "yyyy-MM-dd";
            dateEdit.OnFailure = DateFormatTransformation.FailAction.ReplaceWithNull;
            dateEdit.RangeStart = new DateTime(1999, 5, 3);
            dateEdit.RangeEnd = DateTime.Today;
            edits.Add(dateEdit.ToTransformation());
            Job job = new Job(imports.ToArray(), exports.ToArray(), edits.ToArray());

            // act
            string xml = job.ToXml();
            Job testJob = Job.Deserialize(xml);

            // assert
            // check imports
            for (int n = 0; n < job.Imports.Length; n++)
            {
                Assert.AreEqual(job.Imports[n].Encoding, testJob.Imports[n].Encoding);
                Assert.AreEqual(job.Imports[n].File.FullName, testJob.Imports[n].File.FullName);

                if (job.Imports[n].GetType().Equals(typeof(DatImport)))
                {
                    Assert.AreEqual(
                        ((DatImport)job.Imports[n]).KeyColumnName,
                        ((DatImport)testJob.Imports[n]).KeyColumnName);
                    Assert.AreEqual(
                        ((DatImport)job.Imports[n]).ParentColumnName,
                        ((DatImport)testJob.Imports[n]).ParentColumnName);
                    Assert.AreEqual(
                        ((DatImport)job.Imports[n]).ChildColumnName,
                        ((DatImport)testJob.Imports[n]).ChildColumnName);
                    Assert.AreEqual(
                        ((DatImport)job.Imports[n]).ChildColumnDelimiter,
                        ((DatImport)testJob.Imports[n]).ChildColumnDelimiter);
                    Assert.AreEqual(
                        ((DatImport)job.Imports[n]).HasHeader,
                        ((DatImport)testJob.Imports[n]).HasHeader);
                    Assert.IsTrue(Enumerable.SequenceEqual(
                        ((DatImport)job.Imports[n]).FolderPrependFields,
                        ((DatImport)testJob.Imports[n]).FolderPrependFields));
                    Assert.IsTrue(Enumerable.SequenceEqual(
                        ((DatImport)job.Imports[n]).FolderPrependLinks,
                        ((DatImport)testJob.Imports[n]).FolderPrependLinks));

                    for (int i = 0; i < ((DatImport)job.Imports[n]).LinkedFiles.Length; i++)
                    {
                        Assert.AreEqual(
                            ((DatImport)job.Imports[n]).LinkedFiles[i].ColumnName,
                            ((DatImport)testJob.Imports[n]).LinkedFiles[i].ColumnName);
                        Assert.AreEqual(
                            ((DatImport)job.Imports[n]).LinkedFiles[i].FileType,
                            ((DatImport)testJob.Imports[n]).LinkedFiles[i].FileType);
                    }
                }
                else
                {
                    Assert.AreEqual(
                        ((ImgImport)job.Imports[n]).TextBuilder.FileLevel,
                        ((ImgImport)testJob.Imports[n]).TextBuilder.FileLevel
                        );
                    Assert.AreEqual(
                        ((ImgImport)job.Imports[n]).TextBuilder.FileLocation,
                        ((ImgImport)testJob.Imports[n]).TextBuilder.FileLocation
                        );
                    Assert.AreEqual(
                        ((ImgImport)job.Imports[n]).TextBuilder.PathFind.ToString(),
                        ((ImgImport)testJob.Imports[n]).TextBuilder.PathFind.ToString()
                        );
                    Assert.AreEqual(
                        ((ImgImport)job.Imports[n]).TextBuilder.PathReplace,
                        ((ImgImport)testJob.Imports[n]).TextBuilder.PathReplace
                        );
                }
            }
            // check edits
            for (int i = 0; i < job.Edits.Length; i++)
            {
                Transformation editA = job.Edits[i].ToTransformation();

                if (editA.GetType().Equals(typeof(MetaDataTransformation)))
                {
                    MetaDataTransformation a = (MetaDataTransformation)editA;
                    MetaDataTransformation b = (MetaDataTransformation)testJob.Edits[i].ToTransformation();
                    Assert.AreEqual(a.AlternateDestinationField, b.AlternateDestinationField);
                    Assert.AreEqual(a.AppendField, b.AppendField);
                    Assert.AreEqual(a.FieldName, b.FieldName);
                    Assert.AreEqual(a.FilterField, b.FilterField);
                    Assert.AreEqual(
                        (a.FilterText != null) ? a.FilterText.ToString() : null, 
                        (b.FilterText != null) ? b.FilterText.ToString() : null);
                    Assert.AreEqual(
                        (a.FindText != null) ? a.FindText.ToString() : null, 
                        (b.FindText != null) ? b.FindText.ToString() : null);
                    Assert.AreEqual(a.JoinDelimiter, b.JoinDelimiter);
                    Assert.AreEqual(
                        (a.PrependDirectory != null) ? a.PrependDirectory.FullName : null, 
                        (a.PrependDirectory != null) ? b.PrependDirectory.FullName : null);
                    Assert.AreEqual(a.PrependField, b.PrependField);
                    Assert.AreEqual(a.ReplaceText, b.ReplaceText);
                }
                else if (editA.GetType().Equals(typeof(RepresentativeTransformation)))
                {
                    RepresentativeTransformation a = (RepresentativeTransformation)editA;
                    RepresentativeTransformation b = (RepresentativeTransformation)testJob.Edits[i].ToTransformation();
                    Assert.AreEqual(a.FilterField, b.FilterField);
                    Assert.AreEqual(a.FilterText.ToString(), b.FilterText.ToString());
                    Assert.AreEqual(a.FindText.ToString(), b.FindText.ToString());
                    Assert.AreEqual(a.FindText.Options, b.FindText.Options);
                    Assert.AreEqual(a.NewType, b.NewType);
                    Assert.AreEqual(a.ReplaceText, b.ReplaceText);
                    Assert.AreEqual(a.TargetType, b.TargetType);
                }
                else if (editA.GetType().Equals(typeof(DateFormatTransformation)))
                {
                    DateFormatTransformation a = (DateFormatTransformation)editA;
                    DateFormatTransformation b = (DateFormatTransformation)testJob.Edits[i].ToTransformation();
                    Assert.AreEqual(a.FieldName, b.FieldName);
                    Assert.AreEqual(a.FilterField, b.FilterField);
                    Assert.AreEqual(a.FilterText, b.FilterText);
                    Assert.AreEqual(a.FindText, b.FindText);
                    Assert.AreEqual(a.InputFormat, b.InputFormat);
                    Assert.AreEqual(a.InputTimeZone, b.InputTimeZone);
                    Assert.AreEqual(a.OnFailure, b.OnFailure);
                    Assert.AreEqual(a.OutputFormat, b.OutputFormat);
                    Assert.AreEqual(a.OutputTimeZone, b.OutputTimeZone);
                    Assert.AreEqual(a.RangeEnd, b.RangeEnd);
                    Assert.AreEqual(a.RangeStart, b.RangeStart);
                    Assert.AreEqual(a.ReplaceText, b.ReplaceText);
                }
                else
                {
                    throw new Exception("Invalid type.");
                }
            }
            // check exports
            for (int i = 0; i < job.Exports.Length; i++)
            {
                Assert.AreEqual(job.Exports[i].File.FullName, testJob.Exports[i].File.FullName);
                Assert.AreEqual(job.Exports[i].Encoding, testJob.Exports[i].Encoding);

                if (job.Exports[i].GetType().Equals(typeof(DatExport)))
                {
                    Assert.AreEqual(
                    String.Join(",", ((DatExport)job.Exports[i]).ExportFields),
                    String.Join(",", ((DatExport)testJob.Exports[i]).ExportFields));
                    Assert.AreEqual(
                        ((DatExport)job.Exports[i]).Delimiters.FieldSeparator,
                        ((DatExport)testJob.Exports[i]).Delimiters.FieldSeparator);
                    Assert.AreEqual(
                        ((DatExport)job.Exports[i]).Delimiters.TextQualifier,
                        ((DatExport)testJob.Exports[i]).Delimiters.TextQualifier);
                    Assert.AreEqual(
                        ((DatExport)job.Exports[i]).Delimiters.EscapeCharacter,
                        ((DatExport)testJob.Exports[i]).Delimiters.EscapeCharacter);
                    Assert.AreEqual(
                        ((DatExport)job.Exports[i]).Delimiters.NewRecord,
                        ((DatExport)testJob.Exports[i]).Delimiters.NewRecord);
                    Assert.AreEqual(
                        ((DatExport)job.Exports[i]).Delimiters.FlattenedNewLine,
                        ((DatExport)testJob.Exports[i]).Delimiters.FlattenedNewLine);
                }
                else if (job.Exports[i].GetType().Equals(typeof(XlsExport)))
                {
                    Assert.AreEqual(
                        String.Join(",", ((XlsExport)job.Exports[i]).ExportFields),
                        String.Join(",", ((XlsExport)testJob.Exports[i]).ExportFields));
                    Assert.AreEqual(
                        ((XlsExport)job.Exports[i]).FilePath,
                        ((XlsExport)testJob.Exports[i]).FilePath);

                    for (int j = 0; j < ((XlsExport)job.Exports[i]).Hyperlinks.Length; j++)
                    {
                        Hyperlink link = ((XlsExport)job.Exports[i]).Hyperlinks[j];
                        Hyperlink testLink = ((XlsExport)job.Exports[i]).Hyperlinks[j];

                        Assert.AreEqual(link.FileType, testLink.FileType);
                        Assert.AreEqual(link.DisplayText, testLink.DisplayText);
                        Assert.AreEqual(link.ColumnIndex, testLink.ColumnIndex);
                    }
                }
                else if (job.Exports[i].GetType().Equals(typeof(XrefExport)))
                {
                    XrefExport actual = (XrefExport)testJob.Exports[i];
                    XrefExport expected = (XrefExport)job.Exports[i];
                    Assert.AreEqual(expected.Encoding, actual.Encoding);
                    Assert.AreEqual(expected.FilePath, actual.FilePath);
                    Assert.AreEqual(expected.CustomerDataField, actual.CustomerDataField);
                    Assert.AreEqual(expected.NamedFolderField, actual.NamedFolderField);
                    Assert.AreEqual(expected.NamedFileField, actual.NamedFileField);
                    Assert.IsTrue(expected.CodeStartTrigger.Equals(actual.CodeStartTrigger));
                    Assert.IsTrue(expected.GroupStartTrigger.Equals(actual.GroupStartTrigger));
                    Assert.IsTrue(expected.BoxTrigger.Equals(actual.BoxTrigger));
                    Assert.IsTrue(expected.Slipsheets.Equals(actual.Slipsheets));
                }
                else
                {
                    Assert.AreEqual(
                        ((ImgExport)job.Exports[i]).VolumeName,
                        ((ImgExport)testJob.Exports[i]).VolumeName);
                }                
            }
        }                
    }
}
