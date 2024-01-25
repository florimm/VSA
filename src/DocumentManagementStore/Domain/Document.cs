using DocumentManagementStore.Common.Core.ES;
using DocumentManagementStore.Domain.Events;

namespace DocumentManagementStore.Domain
{
    public class Document : Aggregate
    {
        public Document()
        {

        }

        public Document(string Id)
        {
            var @event = new DocumentCreated(Id);
            ApplyEvent(@event);
        }

        public void MoveDocument(string newFolderId)
        {
            var @event = new DocumentMoved(Id, newFolderId);
            ApplyEvent(@event);
        }

        public void MarkAsDeleted()
        {
            var @event = new DocumentDeleted(Id);
            ApplyEvent(@event);
        }

        public void Rename(string newName)
        {
            var @event = new DocumentRenamed(Id, newName);
            ApplyEvent(@event);
        }

        public void AddMetadata(string key, string val)
        {
            var @event = new DocumentMetadataAdded(Id, key, val);
            ApplyEvent(@event);
        }

        public void RemoveMetadata(string key)
        {
            var @event = new DocumentMetadataRemoved(Id, key);
            ApplyEvent(@event);
        }

        public string Name { get; private set; }

        public string FolderId { get; private set; }

        public bool MarkedAsDeleted { get; set; } = false;

        public List<string> Tags { get; private set; }

        public List<Metadata> Metadata { get; private set; }

        private void Apply(DocumentCreated @event)
        {
            Id = @event.Id;
            Version++;
        }

        private void Apply(DocumentMetadataAdded @event)
        {
            Metadata.Add(new Metadata(@event.Key, @event.Value));
            Version++;
        }

        private void Apply(DocumentMetadataRemoved @event)
        {
            Metadata = Metadata.Where(t => t.Key != @event.Key).ToList();
            Version++;
        }

        private void Apply(DocumentMoved @event)
        {
            FolderId = @event.NewFolderId;
            Version++;
        }

        private void Apply(DocumentDeleted _)
        {
            MarkedAsDeleted = true;
            Version++;
        }

        private void Apply(DocumentRenamed @event)
        {
            Name = @event.Name;
            Version++;
        }
    }

    public class DocumentId(string Name, string Extension) : ValueObject
    {
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
            yield return Extension;
        }
    }

    public record Metadata(string Key, string Value);
}
