var builder = WebApplication.CreateBuilder(args);

// Đăng ký connection string vào DI container
builder.Services.AddSingleton(builder.Configuration.GetConnectionString("DefaultConnection")!);

// Đăng ký Session cho đăng nhập
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

// Cấu hình Localization tiếng Việt (để tự động định dạng ngày dd/MM/yyyy)
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new System.Globalization.CultureInfo("vi-VN") };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("vi-VN");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Áp dụng Localization
app.UseRequestLocalization();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TaiKhoan}/{action=DangNhap}/{id?}");

app.Run();
