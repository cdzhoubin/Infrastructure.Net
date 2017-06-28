using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web.UI;
using Zhoubin.Infrastructure.Web;

// 有关程序集的一般信息由以下
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("Web")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("Web")]

// 将 ComVisible 设置为 false 会使此程序集中的类型
//对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型
//请将此类型的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("7aa5ac5d-92e3-4180-aa2d-6b4a04fb3490")]

// 程序集的版本信息由下列四个值组成: 
//
//      主版本
//      次版本
//      生成号
//      修订号
//
// 可以指定所有值，也可以使用以下所示的 "*" 预置版本号和修订号
//通过使用 "*"，如下所示:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: WebResource(ConstHelper.ValidFilePath, "application/x-javascript")]
[assembly: WebResource(ConstHelper.FileUploadPath, "application/x-javascript")]
[assembly: WebResource(ConstHelper.AlertJavaScriptFilePath, "application/x-javascript")]
[assembly: WebResource(ConstHelper.AlertCssPath, "text/css")]
[assembly: WebResource(ConstHelper.FileUploadPath2, "application/x-javascript")]
