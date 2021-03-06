﻿using Silent.Practices.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Silent.Practices.DDD
{
    public class MemoryEventAggregateRepository<TEntity> : IEventSourcedRepository<TEntity, Guid> where TEntity : EventAggregate<Guid>, new()
    {
        private readonly IEventStore<Guid, Event<Guid>> _eventStore;

        public MemoryEventAggregateRepository(IEventStore<Guid, Event<Guid>> eventStore)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        }

        public virtual Task<TEntity> FindByIdAsync(Guid id)
        {
            IEnumerable<Event<Guid>> committed = _eventStore.GetEventsById(id);
            TEntity eventAggregate = CreateAggregateFromHistory(committed);            
            return Task.FromResult(eventAggregate);
        }

        public virtual Task<ICollection<TEntity>> GetAllAsync()
        {
            ICollection<TEntity> entities = _eventStore.GetEvents()
                .GroupBy(x => x.EntityId)
                .Select(events => CreateAggregateFromHistory(events))
                .ToList();
            return Task.FromResult(entities);
        }

        public virtual Task<bool> SaveAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            IReadOnlyCollection<Event<Guid>> uncommitted = entity.GetUncommitted();
            bool successfull = uncommitted.Any() && _eventStore.SaveEvents(entity.EntityId, uncommitted);
            return Task.FromResult(successfull);
        }

        public virtual async Task<bool> DeleteByIdAsync(Guid key)
        {
            TEntity entity = await FindByIdAsync(key);
            entity?.Archive();
            return true;
        }

        public Task<bool> DeleteAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity?.Archive();
            return Task.FromResult(true);
        }

        private TEntity CreateAggregateFromHistory(IEnumerable<Event<Guid>> events)
        {
            if (events != null && events.Any())
            {
                TEntity eventAggregate = new TEntity();
                eventAggregate.ApplyHistory(events);
                return eventAggregate;
            }

            return null;
        }
    }
}