using DocumentManagementStore.Domain.Events;
using Marten.Events.Projections;

namespace DocumentManagementStore.Projections
{
    public class DocumentReadModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class DocumentReadModelBuilder : MultiStreamProjection<DocumentReadModel, string>
    {
        public DocumentReadModelBuilder()
        {
            this.Identity<DocumentCreated>(e => e.Id);
            this.Identity<DocumentDeleted>(e => e.Id);
        }
        public void Apply(DocumentCreated e, DocumentReadModel model)
        {
            model.Id = e.Id;
            model.Name = e.Name;
        }
        public void Apply(DocumentDeleted e, DocumentReadModel model)
        {
            model.IsDeleted = true;
        }
    }
}
