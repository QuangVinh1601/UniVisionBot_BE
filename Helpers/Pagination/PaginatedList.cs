namespace UniVisionBot.Helpers.Pagination
{
    public class PaginatedList<T> : List<T>
    {
        private const int PAGE_SIZE = 10; 
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        

        public PaginatedList(int totalItems, List<T> items, int indexPage)
        {
            this.AddRange(items);
            TotalPage = (int)Math.Ceiling(totalItems/ (double)PAGE_SIZE);
            CurrentPage = indexPage;
            TotalItems = totalItems;    
            
        }

        public static PaginatedList<T> Pagination( List<T> listItems, int indexPage)
        {
            var totalItems = listItems.Count;
            var items = listItems.Skip((indexPage - 1 )*PAGE_SIZE).Take(PAGE_SIZE).ToList();
            return new PaginatedList<T>(totalItems, items, indexPage);
        }
    }
}
