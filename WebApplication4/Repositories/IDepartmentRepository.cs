using Models;

namespace WebApplication4.Repositories
{
    public interface IDepartmentRepository
    {

        IEnumerable<Department> GetDepartments();
        Department GetDepartment(int departmentId);

    }
}
