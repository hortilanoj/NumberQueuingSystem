namespace NumberQueuingSystem.RPT
{

	/// <summary>
	/// Summary description for rptQueueTicket
	/// </summary>
	public partial class rptQueueTicket : Telerik.Reporting.Report
	{
		public rptQueueTicket()
		{
			//
			// Required for telerik Reporting designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public rptQueueTicket(BOL.Transaction_Queue.objTransactionQueue trans_queue)
		{
			InitializeComponent();

			txtTransacrion.Value = trans_queue.objTransaction.name;
			txtNumber.Value = trans_queue.ToString();
		}
	}
}