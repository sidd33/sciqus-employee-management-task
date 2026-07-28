using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.BUSINESS.BusinessModels
{
	public class PagedResponse<T>
	{
		// The data for the current page
		public IEnumerable<T> Items { get; set; } = new List<T>();

		// Current page number
		public int PageNumber { get; set; }

		// Number of records per page
		public int PageSize { get; set; }

		// Total records available
		public int TotalCount { get; set; }

		// Total number of pages
		public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
	}
}