using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore.Query;

namespace QueryableExtensions
{
  public enum SortOrder
  {
    Asc = 0,
    Des = 1
  }

  public static class QueryableExtensions
  {
    const int PAGE_SIZE_DEFAULT = 10;

    public static (IQueryable<T> pagedQuery, Func<int> countTotal, Indexs) QueryPage<T>(
      this IQueryable<T> baseQuery,
      int? page = 1,
      int? size = PAGE_SIZE_DEFAULT,
      Expression<Func<T, bool>> filter = null,
      Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null
    ) where T : class
    {
      if (!page.HasValue)
        throw new ArgumentNullException(nameof(page));

      if (!size.HasValue)
        throw new ArgumentNullException(nameof(size));
      
      var filteredAndIncludedQuery = FilterAndInclude(baseQuery, filter, includes);

      var pagedFilteredAndIncludedQuery = Paginate(filteredAndIncludedQuery, size.Value, (page.Value - 1) * size.Value);
      
      int countTotal() => filteredAndIncludedQuery.Count();

      var index = Index(countTotal(), page, size);
      
      return (pagedQuery: pagedFilteredAndIncludedQuery, countTotal,index);
    }

    //MODIFICACION
    public static(IQueryable<ProjectT> pagedQuery, Func<int> countTotal, Indexs) QueryPage<T, ProjectT>(
      this IQueryable<T> baseQuery,
      int? page = 1,
      int? size = PAGE_SIZE_DEFAULT,
      Expression<Func<T, bool>> filter = null,
      Expression<Func<T, ProjectT>> project = null,
      params KeyValuePair<Expression<Func<T, object>>, SortOrder >[] orderBys
    ) where T : class
    {
      var sortedQuery = Sort(baseQuery, orderBys);

      var (sortedPagedFilteredAndIncludedQuery, countTotal,index) = QueryPage(sortedQuery, page, size, filter);

      return (pagedQuery: sortedPagedFilteredAndIncludedQuery.Select(project), countTotal,index);
    }
    //MODIFICACION
  
    private static IQueryable<T> Paginate<T>(IQueryable<T> baseQuery, int size, int skipCount) =>
      skipCount == 0 ?
      baseQuery.Take(size) :
      baseQuery.Skip(skipCount).Take(size);

    private static IQueryable<T> FilterAndInclude<T>(
      IQueryable<T> baseQuery,
      Expression<Func<T, bool>> filter,
      Func<IQueryable<T>, IIncludableQueryable<T, object>> includes
      ) where T : class
    {
      var queryIncluded = Include(baseQuery, includes);

      return filter != null ? queryIncluded.Where(filter) : queryIncluded;
    }

    private static IQueryable<T> Sort<T>(
      IQueryable<T> baseQuery,
      KeyValuePair<Expression<Func<T, object>>, SortOrder >[] orderBys
      )
    {
      if (orderBys != null && orderBys.Length > 0)
      {
        foreach (var ordering in orderBys)
        {
          baseQuery = ordering.Value == SortOrder.Asc ?  baseQuery.OrderBy(ordering.Key) : baseQuery.OrderByDescending(ordering.Key);
        }
      }

      return baseQuery;
    }

    private static IQueryable<T> Include<T>(
      IQueryable<T> baseQuery,
      Func<IQueryable<T>, IIncludableQueryable<T, object>> includes
      ) => 
      includes?.Invoke(baseQuery) ?? baseQuery;

    private static Indexs Index(int countTotal,int? page,int? numElementos)
    {
      int primerIndice = 0, ultimoIndice = 0;
      var numElementosTotal = countTotal;
      int? elementosImpares = numElementosTotal % numElementos;
      int? totalPaginas = numElementosTotal / numElementos;
      totalPaginas = elementosImpares > 0 ? totalPaginas + 1 : totalPaginas;

      if (totalPaginas < page - 1)
        page = 0;
      
      int? ultimoElementoPaginaActual = page * numElementos;

      primerIndice = (int)(ultimoElementoPaginaActual - numElementos) + 1;
      
      if (ultimoElementoPaginaActual <= numElementosTotal)
        ultimoIndice = (int)ultimoElementoPaginaActual;
      else
        ultimoIndice = numElementosTotal;

      return new Indexs
      {
        totalPaginas = (int)totalPaginas,
        primerIndice = primerIndice,
        ultimoIndice = ultimoIndice
      };
    }
    
    public record Indexs
    {
      public int totalPaginas { get; set; }
      public int primerIndice{ get; set; }
      public int ultimoIndice { get; set; }
    }
  }
  
  
}
