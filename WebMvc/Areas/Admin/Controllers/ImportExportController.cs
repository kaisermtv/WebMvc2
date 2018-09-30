using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Script.Serialization;
using WebMvc.Application;
using WebMvc.Application.Context;
using WebMvc.Application.CustomActionResults;
using WebMvc.Application.Entities;
using WebMvc.Application.General;
using WebMvc.Application.Interfaces;
using WebMvc.Application.Lib;
using WebMvc.Areas.Admin.ViewModels;
using WebMvc.Services;

namespace WebMvc.Areas.Admin.Controllers
{
    public class ImportExportController : BaseAdminController
    {
        public ImportExportController(LoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, MembershipService membershipService, SettingsService settingsService, LocalizationService localizationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, localizationService)
        {

        }

        // GET: Admin/ImportExport
        public ActionResult Index()
        {
            return View();
        }

        #region Private methods

        /// <summary>
        /// Convert an import report into JSON data
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        private static object ToJSON(CsvReport report)
        {
            var oSerializer = new JavaScriptSerializer();
            var json = new
            {
                HasErrors = report.Errors.Any(),
                HasWarnings = report.Warnings.Any(),
                Warnings = oSerializer.Serialize(report.Warnings.ExtractMessages()),
                Errors = oSerializer.Serialize(report.Errors.ExtractMessages()),
            };

            return json;
        }


        #endregion

        [ChildActionOnly]
        public PartialViewResult Languages()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var importExportViewModel = new LanguagesHomeViewModel();

                // For languages we need a list of export languages and import languages
                var languageImportExportViewModel = new LanguageImportExportViewModel
                {
                    ExportLanguages = LocalizationService.LanguagesInDb,
                    ImportLanguages = LocalizationService.LanguagesAll
                };
                importExportViewModel.LanguageViewModel = languageImportExportViewModel;

                return PartialView(importExportViewModel);
            }
        }

        /// <summary>
        /// Export a language in csv format
        /// </summary>
        /// <param name="languageCulture"></param>
        /// <returns></returns>
        public CsvFileResult ExportLanguage(string languageCulture)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var csv = new CsvFileResult();

                var language = LocalizationService.GetLanguageByLanguageCulture(languageCulture);

                if (language != null)
                {
                    csv.FileDownloadName = languageCulture + ".csv";
                    csv.Body = LocalizationService.ToCsv(language);
                }
                else
                {
                    csv.Body = "No such language";
                    LoggingService.Error("No such language when trying to export language");
                }

                return csv;
            }
        }

        /// <summary>
        /// Post of data for language import (file info)
        /// </summary>
        /// <param name="languageCulture">This defines the name etc of the imported language</param>
        /// <param name="file">The name-value pairs for the language content</param>
        /// <returns></returns>
        [HttpPost]
        public WrappedJsonResult ImportLanguage(string languageCulture, HttpPostedFileBase file)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var report = new CsvReport();

                //http://www.dustinhorne.com/post/2011/11/16/AJAX-File-Uploads-with-jQuery-and-MVC-3.aspx
                try
                {
                    // Verify that the user selected a file
                    if (file != null && file.ContentLength > 0)
                    {
                        // Unpack the data
                        var allLines = new List<string>();
                        using (var streamReader = new StreamReader(file.InputStream, System.Text.Encoding.UTF8, true))
                        {
                            while (streamReader.Peek() >= 0)
                            {
                                allLines.Add(streamReader.ReadLine());
                            }
                        }

                        // Read the CSV file and generate a language
                        report = LocalizationService.FromCsv(languageCulture, allLines);
                        unitOfWork.Commit();
                    }
                    else
                    {
                        report.Errors.Add(new CsvErrorWarning
                        {
                            ErrorWarningType = CsvErrorWarningType.BadDataFormat,
                            Message = "File does not contain a language."
                        });
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    report.Errors.Add(new CsvErrorWarning
                    {
                        ErrorWarningType = CsvErrorWarningType.GeneralError,
                        Message = string.Format("Unable to import language: {0}", ex.Message)
                    });
                }

                return new WrappedJsonResult { Data = ToJSON(report) };
            }
        }

    }
}