/* ----------------------------------------------------------- */
/* Add 2 views used in login/role management                   */
/* ----------------------------------------------------------- */
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vwUserList] AS
SELECT u.UserName, CASE WHEN ur.RoleId IS NULL THEN 0 ELSE 1 END AS IsSuperUser, u.Id
FROM AspNetUsers u 
LEFT OUTER JOIN AspNetUserRoles ur ON u.Id = ur.UserId AND ur.RoleId = 2
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vwUserRoles] AS
	SELECT u.UserName, r.Name AS RoleName
	FROM AspNetUsers u
	INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
	INNER JOIN AspNetRoles r ON r.Id = ur.RoleId
GO

