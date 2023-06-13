using helix.Data;
using helix.Models;
using Microsoft.AspNetCore.Identity;

namespace helix;
public static class SeedData
{


    public static async Task InitialAsync(IServiceProvider services)
    {
        await AddTestData(services.GetRequiredService<ApplicationDbContext>(),
        services.GetRequiredService<UserManager<User>>(),
        services.GetRequiredService<RoleManager<IdentityRole>>());
    }

    public static async Task AddTestData(
        ApplicationDbContext context,
        UserManager<User> _UserManager,
        RoleManager<IdentityRole> _RoleManager
    )
    {
        if (!context.Roles.Any())
        {

            var _adminRole = await _RoleManager.FindByNameAsync("ADMIN");
            if (_adminRole == null)
            {
                await _RoleManager.CreateAsync(new IdentityRole() { Name = "ADMIN" });
            }

            var _generalRole = await _RoleManager.FindByNameAsync("GENERAL");
            if (_generalRole == null)
            {
                await _RoleManager.CreateAsync(new IdentityRole() { Name = "GENERAL" });
            }

            var _operatorRole = await _RoleManager.FindByNameAsync("OPERATOR");
            if (_operatorRole == null)
            {
                await _RoleManager.CreateAsync(new IdentityRole() { Name = "OPERATOR" });
            }
        }


        if (!context.Users.Any())
        {
            var newUser = new User()
            {
                FirstName = "porya",
                LastName = "ras",
                UserName = "admin",
                Email = "admin@hilex.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                Institution = "",
                Type = UserType.ADMIN.ToString()
            };
            var result = await _UserManager.CreateAsync(newUser, "@dm!N123");
            if (result.Succeeded)
            {
                var _user = await _UserManager.FindByIdAsync(newUser.Id);
                if (_user != null)
                {
                    var roleresult = await _UserManager.AddToRoleAsync(_user, UserType.ADMIN.ToString());
                }
            }
        }

    }
}