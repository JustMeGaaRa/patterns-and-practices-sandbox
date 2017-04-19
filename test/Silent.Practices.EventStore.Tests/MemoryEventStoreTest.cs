﻿using System;
using System.Collections.Generic;
using Xunit;

namespace Silent.Practices.EventStore.Tests
{
    public class MemoryEventStoreTest
    {
        [Fact]
        public void SaveEvents_NullObject_ShouldThrowException()
        {
            // Arrange
            IEventStore eventStore = new MemoryEventStore();

            // Act, Assert
            Assert.Throws<ArgumentNullException>(() => eventStore.SaveEvents(1, null));
        }

        [Fact]
        public void SaveEvents_WithEmptyArray_ShouldReturnTrue()
        {
            // Arrange
            Event[] fakeEvents = {};
            IEventStore eventStore = new MemoryEventStore();

            // Act
            bool result = eventStore.SaveEvents(1, fakeEvents);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void SaveEvents_WithNotEmptyArray_ShouldReturnTrue()
        {
            // Arrange
            Event[] fakeEvents = { new FakeEvent() };
            IEventStore eventStore = new MemoryEventStore();

            // Act
            bool result = eventStore.SaveEvents(1, fakeEvents);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GetEventsById_OnEmptyStore_ShouldReturnEmptyCollection()
        {
            // Arrange
            IEventStore eventStore = new MemoryEventStore();

            // Act
            IReadOnlyCollection<Event> result = eventStore.GetEventsById(1);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetEventsById_WithUnexistingId_ShouldReturnEmptyCollection()
        {
            // Arrange
            uint eventAggregateId = 1;
            Event[] fakeEvents = { new FakeEvent() };
            IEventStore eventStore = new MemoryEventStore();
            eventStore.SaveEvents(eventAggregateId, fakeEvents);

            // Act
            IReadOnlyCollection<Event> result = eventStore.GetEventsById(2);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetEventsById_WithExistingId_ShouldReturnEventCollection()
        {
            // Arrange
            uint eventAggregateId = 1;
            Event[] fakeEvents = { new FakeEvent() };
            IEventStore eventStore = new MemoryEventStore();

            // Act
            eventStore.SaveEvents(eventAggregateId, fakeEvents);
            IEnumerable<Event> result = eventStore.GetEventsById(eventAggregateId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        private class FakeEvent : Event
        {
        }
    }
}