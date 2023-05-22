using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Repositories.EFCore.Extensions
{
    public static class BookRepositoryExtensions
    {
        public static IQueryable<Book> FilterBooks(this IQueryable<Book> query,
            uint minPrice, uint macPrice) => query.Where(book =>
            book.Price >= minPrice &&
            book.Price <= macPrice);

        public static IQueryable<Book> Search(this IQueryable<Book> books, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return books;

            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return books
                .Where(b => b.Title
                .ToLower()
                .Contains(searchTerm));
        }

        public static IQueryable<Book> Sort(this IQueryable<Book> books, string OrderByQueryString)
        {
            if (!string.IsNullOrWhiteSpace(OrderByQueryString))
                return books.OrderBy(by => by.Id);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Book>(OrderByQueryString);

            if (orderQuery is null)
                return books.OrderBy(b => b.Id);

            return books.OrderBy(orderQuery);

        }
    }
}
