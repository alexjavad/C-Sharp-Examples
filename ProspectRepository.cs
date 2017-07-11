using System;
using BusinessContactWebsiteTemplate.Repositories;
using BusinessContactWebsiteTemplate.DAL;

namespace BusinessContactWebsiteTemplate.Repositories
{
    public class ProspectRepository : RepositoryBase<Prospect>
    {
        public ProspectRepository(letsgrow_databaseEntities context) : base(context)
        {
            if (context == null)
                throw new ArgumentNullException();
        }
    }
}