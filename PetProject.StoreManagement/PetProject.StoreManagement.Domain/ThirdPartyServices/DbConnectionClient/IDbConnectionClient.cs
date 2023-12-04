using System.Data;

namespace PetProject.StoreManagement.Domain.ThirdPartyServices.DbConnectionClient
{
    public interface IDbConnectionClient
    {
        IDbConnection GetDbConnection();
    }
}
