using System;
using System.ComponentModel;
using System.Diagnostics;
using Telerik.Reporting;

namespace NumberQueuingSystem.RPT
{
    public static class Print_Repository
    {
        public static void Print_Queue(BOL.Transaction_Queue.objTransactionQueue trans_queue)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (s, arg) =>
            {
                try
                {

                    rptQueueTicket report = new rptQueueTicket(trans_queue)
                    {
                        Name = "Queue Ticket"
                    };

                    ReportSource report_source = new InstanceReportSource()
                    {
                        ReportDocument = report
                    };

                    // Obtain the settings of the default printer
                    System.Drawing.Printing.PrinterSettings printerSettings
                        = new System.Drawing.Printing.PrinterSettings();

                    // The standard print controller comes with no UI
                    System.Drawing.Printing.PrintController standardPrintController =
                        new System.Drawing.Printing.StandardPrintController();

                    // Print the report using the custom print controller
                    Telerik.Reporting.Processing.ReportProcessor reportProcessor
                        = new Telerik.Reporting.Processing.ReportProcessor();

                    reportProcessor.PrintController = standardPrintController;

                    //reportProcessor.PrintReport(typeReportSource, printerSettings);
                    reportProcessor.PrintReport(report_source, printerSettings);
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry("Report", ex.Message, EventLogEntryType.Error);
                }
            };

            //bw.RunWorkerCompleted += (s, arg) =>
            //{
            //    if (arg.Error == null)
            //    {

            //    }
            //    else
            //        throw new Exception(arg.Error.Message);
            //};
            bw.RunWorkerAsync();
        }
    }
}
