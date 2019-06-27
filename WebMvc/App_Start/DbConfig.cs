using DataLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMvc.Application;
using WebMvc.Services;

namespace WebMvc
{
    class DbConfig : DataConfig
    {
        public string DbIp => "127.0.0.1";

        public string DbUid => "sa";

        public string DbPwd => "123456";

        public string DbName => "testdblib";

        public SqlType SqlType => ConfigurationManager.ConnectionStrings["WebMvcContext"].ProviderName== "System.Data.SqlClient"? SqlType.SqlServer:SqlType.MySql;

        public void DataConfig()
        {
            throw new NotImplementedException();
        }

        public string GetConnectString()
        {
            return ConfigurationManager.ConnectionStrings["WebMvcContext"].ToString();
        }

        public void TablesConfig(TablesConfig tablesConfig)
        {
            tablesConfig.AddTable("tblSetting")
                .AddColumn("STKEY", SqlDbType.NVarChar).PrimaryKey().Required().MaxLength(30)
                .AddColumn("VALUE", SqlDbType.NText)
                .Insert(@"INSERT INTO [dbo].[tblSetting]([STKEY],[VALUE]) VALUES(N'Theme',N'Classiera')")
                .Insert(string.Format(@"INSERT INTO [dbo].[tblSetting]([STKEY],[VALUE]) VALUES(N'{0}',N'{1}')", AppConstants.STLanguageDefault, AppConstants.InsertLanguage.Id));

            tablesConfig.AddTable("MembershipLogin")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("UserId", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("Password", SqlDbType.NVarChar).MaxLength(200)
                .AddColumn("TypeLogin", SqlDbType.Int).Required().Default("0")
                .AddColumn("LoginDate", SqlDbType.DateTime).Required().Default("GETDATE()")
                .AddColumn("OnlineDate", SqlDbType.DateTime).Default("GETDATE()")
                .AddColumn("Remember", SqlDbType.Bit).Required().Default("0");


            tablesConfig.AddTable("MembershipUser")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).Required().PrimaryKey()
                .AddColumn("UserName", SqlDbType.NVarChar).MaxLength(150).Unique().Required()
                .AddColumn("Password", SqlDbType.NVarChar).MaxLength(200).Required()
                .AddColumn("PasswordSalt", SqlDbType.NVarChar).MaxLength(128)
                .AddColumn("Email", SqlDbType.NVarChar).MaxLength(256)
                .AddColumn("PasswordQuestion", SqlDbType.NVarChar).MaxLength(256)
                .AddColumn("PasswordAnswer", SqlDbType.NVarChar).MaxLength(256)
                .AddColumn("IsApproved", SqlDbType.Bit).Required().Default("0")
                .AddColumn("IsLockedOut", SqlDbType.Bit).Required().Default("1")
                .AddColumn("IsBanned", SqlDbType.Bit).Required().Default("0")
                .AddColumn("CreateDate", SqlDbType.DateTime).Required().Default("GETUTCDATE()")
                .AddColumn("LastLoginDate", SqlDbType.DateTime).Required().Default("GETUTCDATE()")
                .AddColumn("LastPasswordChangedDate", SqlDbType.DateTime).Required().Default("GETUTCDATE()")
                .AddColumn("LastLockoutDate", SqlDbType.DateTime).Required().Default("GETUTCDATE()")
                .AddColumn("LastActivityDate", SqlDbType.DateTime)
                .AddColumn("FailedPasswordAttemptCount", SqlDbType.Int).Required().Default("0")
                .AddColumn("FailedPasswordAnswerAttempt", SqlDbType.Int).Required().Default("0")
                .AddColumn("PasswordResetToken", SqlDbType.NVarChar).MaxLength(150)
                .AddColumn("PasswordResetTokenCreatedAt", SqlDbType.DateTime)
                .AddColumn("Comment", SqlDbType.NVarChar)
                .AddColumn("Slug", SqlDbType.NVarChar).Required().MaxLength(256).Unique()
                .AddColumn("Signature", SqlDbType.NVarChar).MaxLength(1000)
                .AddColumn("Age", SqlDbType.Int)
                .AddColumn("Location", SqlDbType.NVarChar).MaxLength(100)
                .AddColumn("Website", SqlDbType.NVarChar).MaxLength(100)
                .AddColumn("Twitter", SqlDbType.NVarChar).MaxLength(60)
                .AddColumn("Facebook", SqlDbType.NVarChar).MaxLength(60)
                .AddColumn("Avatar", SqlDbType.NVarChar).MaxLength(500)
                .AddColumn("FacebookAccessToken", SqlDbType.NVarChar).MaxLength(300)
                .AddColumn("FacebookId", SqlDbType.BigInt)
                .AddColumn("TwitterAccessToken", SqlDbType.NVarChar).MaxLength(300)
                .AddColumn("TwitterId", SqlDbType.BigInt)
                .AddColumn("GoogleAccessToken", SqlDbType.NVarChar).MaxLength(300)
                .AddColumn("GoogleId", SqlDbType.BigInt)
                .AddColumn("MicrosoftAccessToken", SqlDbType.NVarChar).MaxLength(450)
                .AddColumn("MicrosoftId", SqlDbType.NVarChar)
                .AddColumn("IsExternalAccount", SqlDbType.Bit)
                .AddColumn("TwitterShowFeed", SqlDbType.Bit)
                .AddColumn("LoginIdExpires", SqlDbType.DateTime)
                .AddColumn("MiscAccessToken", SqlDbType.NVarChar).MaxLength(250)
                .AddColumn("DisableEmailNotifications", SqlDbType.Bit)
                .AddColumn("DisablePosting", SqlDbType.Bit)
                .AddColumn("DisablePrivateMessages", SqlDbType.Bit)
                .AddColumn("DisableFileUploads", SqlDbType.Bit)
                .AddColumn("HasAgreedToTermsAndConditions", SqlDbType.Bit)
                .AddColumn("Latitude", SqlDbType.NVarChar).MaxLength(40)
                .AddColumn("Longitude", SqlDbType.NVarChar).MaxLength(40)
                .Insert(@"INSERT [dbo].[MembershipUser] ([Id], [UserName], [Password], [PasswordSalt], [Email], [PasswordQuestion], [PasswordAnswer], [IsApproved], [IsLockedOut], [IsBanned], [CreateDate], [LastLoginDate], [LastPasswordChangedDate], [LastLockoutDate], [LastActivityDate], [FailedPasswordAttemptCount], [FailedPasswordAnswerAttempt], [PasswordResetToken], [PasswordResetTokenCreatedAt], [Comment], [Slug], [Signature], [Age], [Location], [Website], [Twitter], [Facebook], [Avatar], [FacebookAccessToken], [FacebookId], [TwitterAccessToken], [TwitterId], [GoogleAccessToken], [GoogleId], [MicrosoftAccessToken], [MicrosoftId], [IsExternalAccount], [TwitterShowFeed], [LoginIdExpires], [MiscAccessToken], [DisableEmailNotifications], [DisablePosting], [DisablePrivateMessages], [DisableFileUploads], [HasAgreedToTermsAndConditions], [Latitude], [Longitude]) "
                         +   @"VALUES(N'687abc70-f93d-49d4-985c-a8af00dccdad', N'admin', N'wIJuGJDB9b/EvK1firQp/gTtjPBEpUy7G+RyJmXUfbY=', N'IBAqGIIeeKFo8TFbVf8M+V9lgSGKH2Et', N'admin@yahoo.com', NULL, NULL, 1, 0, 0, CAST(N'2018-03-26 13:23:55.370' AS DateTime), CAST(N'2018-03-27 09:14:56.693' AS DateTime), CAST(N'2018-03-26 13:23:55.370' AS DateTime), CAST(N'1753-01-01 00:00:00.000' AS DateTime), NULL, 0, 0, NULL, NULL, NULL, N'admin', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)");

            tablesConfig.AddTable("MembershipRole")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("RoleName", SqlDbType.NVarChar).MaxLength(250).Required().Unique()
                .AddColumn("Description", SqlDbType.NVarChar)
                .AddColumn("Lock",SqlDbType.Bit).Required().Default("0")
                .Insert(@"INSERT INTO [dbo].[MembershipRole]([Id],[RoleName],[Description],[Lock])VALUES('1D42BBBA-834F-457F-AF32-A827003B2D1F','Admin',N'Quản trị viên',1)")
                .Insert(@"INSERT INTO [dbo].[MembershipRole]([Id],[RoleName],[Description],[Lock])VALUES('FD95507D-4002-4C5B-8A64-A827003B2D22','Standard Members',N'Thành viên tiêu chuẩn',1)")
                .Insert(@"INSERT INTO [dbo].[MembershipRole]([Id],[RoleName],[Description],[Lock])VALUES('3D2B6D1C-74F4-4ABF-9378-A827003B2D24','Guest',N'Khách vãng lai',1)");


            tablesConfig.AddTable("MembershipUsersInRoles")
                .AddColumn("UserIdentifier", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("RoleIdentifier", SqlDbType.UniqueIdentifier).Required()
                .Insert(@"INSERT [dbo].[MembershipUsersInRoles] ([UserIdentifier], [RoleIdentifier]) VALUES (N'687abc70-f93d-49d4-985c-a8af00dccdad', N'1d42bbba-834f-457f-af32-a827003b2d1f')");

            tablesConfig.AddTable("Permission")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("PermissionId", SqlDbType.UniqueIdentifier)
                .AddColumn("Name", SqlDbType.NVarChar).Required().MaxLength(150).Unique()
                .AddColumn("IsGlobal", SqlDbType.Bit).Required().Default("0")
                .AddColumn("Lock",SqlDbType.Bit).Required().Default("0");

            tablesConfig.AddTable("PermissionsInRoles")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("PermissionId", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("RoleId", SqlDbType.UniqueIdentifier).Required();


            tablesConfig.AddTable("Language")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("Name", SqlDbType.NVarChar).MaxLength(100)
                .AddColumn("LanguageCulture", SqlDbType.NVarChar).MaxLength(20)
                .AddColumn("FlagImageFileName", SqlDbType.NVarChar).MaxLength(50)
                .AddColumn("RightToLeft", SqlDbType.Bit).Required().Default("0")
                .Insert(AppConstants.InsertLanguage);
                //.Insert(@"INSERT INTO [dbo].[Language]([Id],[Name],[LanguageCulture],[FlagImageFileName],[RightToLeft]) VALUES('9B4F55B2-589D-4200-B862-A8270040EA71','Vietnamese (Vietnam)','vi-VN',null,0)");

            tablesConfig.AddTable("LocaleResourceKey")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("Name", SqlDbType.NVarChar).MaxLength(200)
                .AddColumn("Notes", SqlDbType.NVarChar)
                .AddColumn("DateAdded", SqlDbType.DateTime).Required().Default("GETDATE()")
                .Insert(@"INSERT INTO [dbo].[LocaleResourceKey]([Id],[Name],[Notes],[DateAdded]) VALUES('2652D065-4AB8-47CB-94DF-AA7400AD6FE7','Product.Attribute.Price','',GETDATE())")
                .Insert(@"INSERT INTO [dbo].[LocaleResourceKey]([Id],[Name],[Notes],[DateAdded]) VALUES('35DC418E-7598-439B-9B96-AA7400B7D836','Product.Attribute.ProductCode','',GETDATE())")
                .Insert(@"INSERT INTO [dbo].[LocaleResourceKey]([Id],[Name],[Notes],[DateAdded]) VALUES('AD200C05-54FB-4E28-8C88-AA7400ADAFEF','Product.Attribute.Promotion','',GETDATE())");

            tablesConfig.AddTable("LocaleStringResource")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("ResourceValue", SqlDbType.NVarChar).MaxLength(1000)
                .AddColumn("LocaleResourceKey_Id", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("Language_Id", SqlDbType.UniqueIdentifier).Required()
                .Insert(@"INSERT INTO [dbo].[LocaleStringResource]([Id],[ResourceValue],[LocaleResourceKey_Id],[Language_Id]) VALUES('888BFCC7-1B10-4366-B3EF-AA7400AD7C20',N'Giá','2652D065-4AB8-47CB-94DF-AA7400AD6FE7','9B4F55B2-589D-4200-B862-A8270040EA71')")
                .Insert(@"INSERT INTO [dbo].[LocaleStringResource]([Id],[ResourceValue],[LocaleResourceKey_Id],[Language_Id]) VALUES('42B3CB5B-68C1-42A8-83E2-AA7400B7E903',N'Mã sản phẩm','35DC418E-7598-439B-9B96-AA7400B7D836','9B4F55B2-589D-4200-B862-A8270040EA71')")
                .Insert(@"INSERT INTO [dbo].[LocaleStringResource]([Id],[ResourceValue],[LocaleResourceKey_Id],[Language_Id]) VALUES('D80E6B14-C8F4-49DA-B955-AA7400ADC471',N'Giá khuyến mãi','AD200C05-54FB-4E28-8C88-AA7400ADAFEF','9B4F55B2-589D-4200-B862-A8270040EA71')");

            tablesConfig.AddTable("Category")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("Name", SqlDbType.NVarChar).MaxLength(450).Required()
                .AddColumn("Description", SqlDbType.NVarChar)
                .AddColumn("IsLocked", SqlDbType.Bit).Required().Default("0")
                .AddColumn("ModerateTopics", SqlDbType.Bit).Required().Default("0")
                .AddColumn("ModeratePosts", SqlDbType.Bit).Required().Default("0")
                .AddColumn("SortOrder", SqlDbType.Int).Required().Default("0")
                .AddColumn("DateCreated", SqlDbType.DateTime).Required().Default("GETDATE()")
                .AddColumn("Slug", SqlDbType.NVarChar).MaxLength(450).Required().Unique()
                .AddColumn("PageTitle", SqlDbType.NVarChar).MaxLength(80)
                .AddColumn("Path", SqlDbType.NVarChar).MaxLength(2500)
                .AddColumn("MetaDescription", SqlDbType.NVarChar).MaxLength(200)
                .AddColumn("Colour", SqlDbType.NVarChar).MaxLength(50)
                .AddColumn("Image", SqlDbType.NVarChar).MaxLength(500)
                .AddColumn("Category_Id", SqlDbType.UniqueIdentifier)
                .AddColumn("IsProduct", SqlDbType.Bit);

            tablesConfig.AddTable("Menu")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("Menu_Id", SqlDbType.UniqueIdentifier)
                .AddColumn("Name", SqlDbType.NVarChar).MaxLength(450).Required()
                .AddColumn("Image", SqlDbType.NVarChar).MaxLength(500)
                .AddColumn("Description", SqlDbType.NVarChar)
                .AddColumn("Colour", SqlDbType.NVarChar).MaxLength(50)
                .AddColumn("iType", SqlDbType.Int).Required().Default("0")
                .AddColumn("Link", SqlDbType.NVarChar)
                .AddColumn("SortOrder", SqlDbType.Int).Required().Default("0");

            tablesConfig.AddTable("Carousel")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("Carousel_Id", SqlDbType.UniqueIdentifier)
                .AddColumn("Name", SqlDbType.NVarChar).MaxLength(450).Required()
                .AddColumn("Image", SqlDbType.NVarChar).MaxLength(450)
                .AddColumn("Description", SqlDbType.NVarChar)
                .AddColumn("Link", SqlDbType.NVarChar)
                .AddColumn("SortOrder", SqlDbType.Int).Required().Default("0");

            tablesConfig.AddTable("Topic")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("Name", SqlDbType.NVarChar).MaxLength(450).Required()
                .AddColumn("ShotContent", SqlDbType.NVarChar).MaxLength(1000)
                .AddColumn("SEOKeyword", SqlDbType.NVarChar).MaxLength(1000)
                .AddColumn("SEODescription", SqlDbType.NVarChar)
                .AddColumn("Intro", SqlDbType.NVarChar)
                .AddColumn("isAutoShotContent", SqlDbType.Bit).Required().Default("0")
                .AddColumn("Image", SqlDbType.NVarChar)
                .AddColumn("CreateDate", SqlDbType.DateTime).Required().Default("GETDATE()")
                .AddColumn("Solved", SqlDbType.Bit).Required().Default("0")
                .AddColumn("SolvedReminderSent", SqlDbType.Bit)
                .AddColumn("Slug", SqlDbType.NVarChar).MaxLength(1000).Required().Unique()
                .AddColumn("Views", SqlDbType.Int).Required().Default("0")
                .AddColumn("IsSticky", SqlDbType.Bit).Required().Default("0")
                .AddColumn("IsLocked", SqlDbType.Bit).Required().Default("0")
                .AddColumn("Pending", SqlDbType.Bit)
                .AddColumn("Category_Id", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("Post_Id", SqlDbType.UniqueIdentifier)
                .AddColumn("Poll_Id", SqlDbType.UniqueIdentifier)
                .AddColumn("MembershipUser_Id", SqlDbType.UniqueIdentifier).Required();


            tablesConfig.AddTable("Post")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("PostContent", SqlDbType.NVarChar).Required()
                .AddColumn("DateCreated", SqlDbType.DateTime).Required().Default("GETDATE()")
                .AddColumn("VoteCount", SqlDbType.Int).Required().Default("0")
                .AddColumn("DateEdited", SqlDbType.DateTime).Required().Default("GETDATE()")
                .AddColumn("IsSolution", SqlDbType.Bit).Required().Default("0")
                .AddColumn("IsTopicStarter", SqlDbType.Bit).Required().Default("0")
                .AddColumn("FlaggedAsSpam", SqlDbType.Bit)
                .AddColumn("IpAddress", SqlDbType.NVarChar).MaxLength(50)
                .AddColumn("Pending", SqlDbType.Bit)
                .AddColumn("SearchField", SqlDbType.NVarChar)
                .AddColumn("InReplyTo", SqlDbType.UniqueIdentifier)
                .AddColumn("Topic_Id", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("MembershipUser_Id", SqlDbType.UniqueIdentifier).Required();

            tablesConfig.AddTable("ProductClass")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("Name", SqlDbType.NVarChar).MaxLength(450).Required()
                .AddColumn("Description", SqlDbType.NVarChar)
                .AddColumn("IsLocked", SqlDbType.Bit).Required().Default("0")
                .AddColumn("DateCreated", SqlDbType.DateTime).Required().Default("GETDATE()")
                .AddColumn("Slug", SqlDbType.NVarChar).MaxLength(450).Required().Unique()
                .AddColumn("Colour", SqlDbType.NVarChar).MaxLength(50)
                .AddColumn("Image", SqlDbType.NVarChar).MaxLength(450);

            tablesConfig.AddTable("ProductAttribute")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("LangName", SqlDbType.NVarChar).MaxLength(200)
                .AddColumn("ValueType", SqlDbType.Int).Required().Default("0")
                .AddColumn("ValueOption", SqlDbType.NText)
                .AddColumn("ValueFindter", SqlDbType.NText)
                .AddColumn("IsShowFindter", SqlDbType.Bit).Required().Default("0")
                .AddColumn("IsNull", SqlDbType.Bit).Required().Default("0")
                .AddColumn("IsLock", SqlDbType.Bit).Required().Default("0")
                .Insert("INSERT [dbo].[ProductAttribute] ([Id], [LangName], [ValueType], [ValueOption], [ValueFindter], [IsShowFindter], [IsNull], [IsLock]) VALUES (N'944e407a-320f-4b47-b98f-a9f9010994a6', N'Price', 0, N'null', N'null', 0, 1, 1)")
                .Insert("INSERT [dbo].[ProductAttribute] ([Id], [LangName], [ValueType], [ValueOption], [ValueFindter], [IsShowFindter], [IsNull], [IsLock]) VALUES (N'78BED776-285A-4538-A8E2-AA7400B79959', N'ProductCode', 0, N'null', N'null', 0, 1, 1)")
                .Insert("INSERT [dbo].[ProductAttribute] ([Id], [LangName], [ValueType], [ValueOption], [ValueFindter], [IsShowFindter], [IsNull], [IsLock]) VALUES (N'46eb0aae-90da-436f-9018-a9f90109f90e', N'Promotion', 1, N'null', N'null', 0, 1, 1)");

            tablesConfig.AddTable("ProductClassAttribute")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("ProductClassId", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("ProductAttributeId", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("IsShow", SqlDbType.Bit).Required().Default("0");

            tablesConfig.AddTable("Product")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("ProductClassId", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("Name", SqlDbType.NVarChar).MaxLength(1000).Required()
                .AddColumn("ShotContent", SqlDbType.NVarChar).MaxLength(1000)
                .AddColumn("isAutoShotContent", SqlDbType.Bit).Required().Default("0")
                .AddColumn("Image", SqlDbType.NVarChar)
                .AddColumn("CreateDate", SqlDbType.DateTime).Required().Default("GETDATE()")
                .AddColumn("Slug", SqlDbType.NVarChar).MaxLength(1000).Required().Unique()
                .AddColumn("Views", SqlDbType.Int).Required().Default("0")
                .AddColumn("IsLocked", SqlDbType.Bit).Required().Default("0")
                .AddColumn("Pending", SqlDbType.Bit)
                .AddColumn("Category_Id", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("ProductPost_Id", SqlDbType.UniqueIdentifier)
                .AddColumn("MembershipUser_Id", SqlDbType.UniqueIdentifier).Required();

            tablesConfig.AddTable("ProductAttributeValue")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("ProductId", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("ProductAttributeId", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("Value", SqlDbType.NVarChar);

            tablesConfig.AddTable("ProductPost")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("PostContent", SqlDbType.NVarChar)
                .AddColumn("DateCreated", SqlDbType.DateTime).Required().Default("GETDATE()")
                .AddColumn("VoteCount", SqlDbType.Int).Required().Default("0")
                .AddColumn("DateEdited", SqlDbType.DateTime).Required().Default("GETDATE()")
                .AddColumn("IsSolution", SqlDbType.Bit).Required().Default("0")
                .AddColumn("IsTopicStarter", SqlDbType.Bit).Required().Default("0")
                .AddColumn("FlaggedAsSpam", SqlDbType.Bit)
                .AddColumn("IpAddress", SqlDbType.NVarChar).MaxLength(50)
                .AddColumn("Pending", SqlDbType.Bit)
                .AddColumn("SearchField", SqlDbType.NVarChar)
                .AddColumn("InReplyTo", SqlDbType.UniqueIdentifier)
                .AddColumn("Product_Id", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("MembershipUser_Id", SqlDbType.UniqueIdentifier).Required();

           
            tablesConfig.AddTable("Contact")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("Name", SqlDbType.NVarChar).MaxLength(150)
                .AddColumn("Email", SqlDbType.NVarChar).MaxLength(150)
                .AddColumn("Content", SqlDbType.NVarChar)
                .AddColumn("IsCheck", SqlDbType.Bit).Required().Default("0")
                .AddColumn("Note", SqlDbType.NVarChar)
                .AddColumn("CreateDate", SqlDbType.DateTime).Required().Default("GETDATE()");

            tablesConfig.AddTable("TypeRoom")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("Name", SqlDbType.NVarChar).MaxLength(150)
                .AddColumn("IsShow", SqlDbType.Bit).Required().Default("0")
                .AddColumn("Note", SqlDbType.NVarChar)
                .AddColumn("CreateDate", SqlDbType.DateTime).Required().Default("GETDATE()");

            tablesConfig.AddTable("Booking")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("NamePartner", SqlDbType.NVarChar).MaxLength(150)
                .AddColumn("CheckIn", SqlDbType.DateTime)
                .AddColumn("CheckOut", SqlDbType.DateTime)
                .AddColumn("Adukts", SqlDbType.Int)
                .AddColumn("Adolescent", SqlDbType.Int)
                .AddColumn("Children", SqlDbType.Int)
                .AddColumn("TypeRoom_Id", SqlDbType.UniqueIdentifier)
                .AddColumn("Phone", SqlDbType.NVarChar).MaxLength(20)
                .AddColumn("Email", SqlDbType.NVarChar).MaxLength(150)
                .AddColumn("IsCheck", SqlDbType.Bit).Required().Default("0")
                .AddColumn("Note", SqlDbType.NVarChar)
                .AddColumn("CreateDate", SqlDbType.DateTime).Required().Default("GETDATE()");

            tablesConfig.AddTable("ShoppingCart")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("Name", SqlDbType.NVarChar).MaxLength(150)
                .AddColumn("Email", SqlDbType.NVarChar).MaxLength(150)
                .AddColumn("Phone", SqlDbType.NVarChar).MaxLength(20)
                .AddColumn("Addren", SqlDbType.NVarChar).MaxLength(256)
                .AddColumn("ShipName", SqlDbType.NVarChar).MaxLength(150)
                .AddColumn("ShipPhone", SqlDbType.NVarChar).MaxLength(20)
                .AddColumn("ShipAddren", SqlDbType.NVarChar).MaxLength(256)
                .AddColumn("ShipNote", SqlDbType.NVarChar)
                .AddColumn("TotalMoney", SqlDbType.NVarChar).MaxLength(256)
                .AddColumn("Note", SqlDbType.NVarChar)
                .AddColumn("Status", SqlDbType.Int)
                .AddColumn("CreateDate", SqlDbType.DateTime).Required().Default("GETDATE()");

            tablesConfig.AddTable("ShoppingCartProduct")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("ShoppingCartId", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("ProductId", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("Price", SqlDbType.NVarChar).MaxLength(150)
                .AddColumn("CountProduct", SqlDbType.NVarChar).Required();

            tablesConfig.AddTable("EmployeesRole")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("Name", SqlDbType.NVarChar).MaxLength(150).Required()
                .AddColumn("Description", SqlDbType.NVarChar)
                .AddColumn("SortOrder", SqlDbType.Int).Default("0");

            tablesConfig.AddTable("Employees")
                .AddColumn("Id", SqlDbType.UniqueIdentifier).PrimaryKey().Required()
                .AddColumn("RoleId", SqlDbType.UniqueIdentifier).Required()
                .AddColumn("Name", SqlDbType.NVarChar).MaxLength(150).Required()
                .AddColumn("Phone", SqlDbType.NVarChar).MaxLength(20)
                .AddColumn("Skype", SqlDbType.NVarChar).MaxLength(150)
                .AddColumn("Email", SqlDbType.NVarChar).MaxLength(150);

        }
    }
}

