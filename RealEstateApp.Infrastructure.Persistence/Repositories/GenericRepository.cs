﻿using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using System.Linq.Expressions;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : class
    {
        protected readonly RealEstateContext _context;

        public GenericRepository(RealEstateContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Entity, bool>> predicate)
        {
            return await _context.Set<Entity>().AnyAsync(predicate);
        }

        public virtual async Task<Entity?> AddAsync(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");

            await _context.Set<Entity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<Entity?> UpdateAsync(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<Entity?> UpdateAsync(int id, Entity entity)
        {
            var entry = await _context.Set<Entity>().FindAsync(id);

            if (entry != null)
            {
                _context.Entry(entry).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
                return entry;
            }
            return null;
        }

        public virtual async Task DeleteAsync(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");

            _context.Set<Entity>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int? id)
        {
            var entity = await _context.Set<Entity>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<Entity>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<Entity?> GetById(int id)
        {
            try
            {
                return await _context.Set<Entity>().FindAsync(id);
            }
            catch (Exception ex)
            {
                // Puedes registrar el error o manejarlo según tu lógica
                throw new ApplicationException($"An error occurred while fetching the entity by ID: {ex.Message}", ex);
            }
        }

        public virtual async Task<List<Entity>> GetAll()
        {
            try
            {
                return await _context.Set<Entity>().ToListAsync();
            }
            catch (Exception ex)
            {
                // Manejamos el error en caso de fallos en la consulta
                throw new ApplicationException($"An error occurred while fetching all entities: {ex.Message}", ex);
            }
        }

        public virtual async Task<List<Entity>> GetAllWithInclude(List<string> properties)
        {
            var query = _context.Set<Entity>().AsQueryable();

            foreach (var property in properties)
            {
                query = query.Include(property);
            }

            return await query.ToListAsync();
        }

        public virtual IQueryable<Entity> GetAllQuery()
        {
            return _context.Set<Entity>().AsQueryable();
        }

        public virtual IQueryable<Entity> GetAllQueryWithInclude(List<string> properties)
        {
            var query = _context.Set<Entity>().AsQueryable();

            foreach (var property in properties)
            {
                query = query.Include(property);
            }
            return query;
        }

        public virtual async Task<IEnumerable<Entity>> GetByConditionAsync(Expression<Func<Entity, bool>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression), "Expression cannot be null.");

            return await _context.Set<Entity>().Where(expression).ToListAsync();
        }

        public virtual async Task<Entity?> GetByIdWithInclude(int id, List<string> includeProperties)
        {
            IQueryable<Entity> query = _context.Set<Entity>().AsQueryable();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

    }
}
