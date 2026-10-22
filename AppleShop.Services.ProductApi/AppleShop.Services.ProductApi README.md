# AppleShop Product Apis Microservice

This project is a lightweight .NET RESTful microservice for product management, providing full CRUD operations.

## Overview
This service enables:

* Admins to create, read, modify, and delete products.
* Customers (via the web frontend) to view product details.


## Prerequisites

* \[.NET 6+ SDK](https://dotnet.microsoft.com/download)
* SQL Server

  

## Production Deployment
In production, inject the connection string via **environment variables**.

Setting the environment variable (examples)


#### Linux / macOS (bash)
```bash

export ConnectionStrings__AppleShop_ProductApi_DefaultConnection="Data Source=prod_server;Initial Catalog=AppleShop_ProdctApiDb;User Id=prod_user;Password=prod_password;Encrypt=True;"
```

#### Windows (Command Prompt)
```bash
setx ConnectionStrings__AppleShop_ProductApi_DefaultConnection "Data Source=prod_server;Initial Catalog=AppleShop_ProdctApiDb;User Id=prod_user;Password=prod_password;Encrypt=True;"
```

### Execute the Script

#### Review the migrate.sql Script
<u>Before running anything in production, open the migrate.sql file you just created in a text editor. Carefully review it for any unexpected DROP, DELETE, or schema changes. This step is critical for catching potential issues before they cause data loss</u>

This is a common best practice to **wrap your query in a transaction**: For added safety on critical systems, you can manually wrap the entire script in a transaction. ADD <mark> BEGIN TRANSACTION;</mark> at the top and <mark>COMMIT;</mark> at the bottom. If something goes wrong, you can run a <mark>ROLLBACK;</mark> before committing.


#### Execute the Script via **sqlcmd** command-line tool
```bash

sqlcmd -S your_server -d YourDatabase -i migrate.sql
```
#### Execute the Script via SSMS to setting up database

1. Launch Microsoft SQL Server Management Studio.
2. Connect to your production server(the same ones you used for your production environment variable).
3. Select the target database.
4. Execute the script.
5. Verify the Execution (after the script finishes, check the **Messages** tab in SSMS. You should see "Commands completed successfully." and a log of the migrations that were applied. This confirms the update was successful.)




## Development Setup

We use User Secrets to keep sensitive data (like passwords) out of source control.

### Initialize user secrets for the project:
In visual studio right click on project and then select <mark>Manage User Secrets</mark> button.

Alternatively from project terminal run the bellow command
```bash
dotnet user-secrets init
```


Set the connection string (replace placeholders with your actual development SQL Server credentials):
```bash

dotnet user-secrets set "ConnectionStrings:AppleShop_ProductApi_DefaultConnection" "Data Source=localhost,1433;Initial Catalog=AppleShop_ProdctApiDb;Integrated Security=True; MultipleActiveResultSets=True; TrustServerCertificate=True; Encrypt=True;"
```

### Run database migrations (project is using EF-Core)

you can use package manager console and run command 
```

Update-Database
```

or using developer powershell run bellow command
```bash

dotnet ef database update
```

Run the service
```bash

dotnet run
```

 Generate an idempotent SQL script (The --idempotent flag is crucial. It adds logic to the script that checks which migrations have already been applied to the target database, meaning the script can be run safely without causing errors if some changes are already present)
 
 ```bash

 dotnet ef migrations script --idempotent --output migrate.sql
```
