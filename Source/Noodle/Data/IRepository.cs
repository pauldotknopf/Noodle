using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Noodle.Data
{
    /// <summary>
    /// A repository that can be used for LinqToSql, EntityFramework and even nHibernate! Lets keep db-access garbage out of the libraries!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks></remarks>
    public interface IRepository<T> where T : class, new()
    {
        /// <summary>
        /// Get a object by a primary key
        /// </summary>
        /// <typeparam name="TPrimary">The type of the primary.</typeparam>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        T GetById<TPrimary>(TPrimary id);

        /// <summary>
        /// Insert an object.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <remarks></remarks>
        T Insert(T entity);

        /// <summary>
        /// Update an object.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <remarks></remarks>
        T Update(T entity);

        /// <summary>
        /// Delete an object.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <remarks></remarks>
        void Delete(T entity);

        /// <summary>
        /// Delete all objects where the condition is met.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        int DeleteAll(Expression<Func<T, bool>> exp);

        /// <summary>
        /// Delete all objects.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        int DeleteAll();

        /// <summary>
        /// Return an IQueryable for manipulation
        /// </summary>
        /// <remarks></remarks>
        IQueryable<T> Table { get; }

        /// <summary>
        /// Find all objects where the condition is met.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        IEnumerable<T> FindAll(Expression<Func<T, bool>> exp);

        /// <summary>
        /// Grab a single item. Throws an error if more than one object exists.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        T Single(Expression<Func<T, bool>> exp);

        /// <summary>
        /// Grab the first item where the condition is met.
        /// </summary>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        T First(Expression<Func<T, bool>> exp);

        /// <summary>
        /// Create an instance of an object.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        T CreateInstance();

        /// <summary>
        /// Execute a method, typically a stored procedure
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        object ExecuteMethodQuery(MethodInfo method, params object[] parameters);

        /// <summary>
        /// Execute a method, typically a stored procedure
        /// </summary>
        /// <param name="lambda">The lambda.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        TReturn ExecuteMethodQuery<TObject, TReturn>(Expression<Func<TObject, TReturn>> lambda);

        /// <summary>
        /// Execute a method, typically a stored procedure
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks></remarks>
        void ExecuteMethod(MethodInfo method, params object[] parameters);

        /// <summary>
        /// Execute a method, typically a stored procedure
        /// </summary>
        /// <param name="lambda">The lambda.</param>
        /// <remarks></remarks>
        void ExecuteMethod<TObject>(Expression<Action<TObject>> lambda);
    }
}
