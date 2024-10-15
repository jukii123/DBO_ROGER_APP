-- Borrar tablas si existen en el orden correcto
IF OBJECT_ID('dbo.orders', 'U') IS NOT NULL 
    DROP TABLE dbo.orders;

IF OBJECT_ID('dbo.products', 'U') IS NOT NULL 
    DROP TABLE dbo.products;

IF OBJECT_ID('dbo.customers', 'U') IS NOT NULL 
    DROP TABLE dbo.customers;

IF OBJECT_ID('dbo.users', 'U') IS NOT NULL 
    DROP TABLE dbo.users;


    CREATE TABLE users (
    id INT PRIMARY KEY IDENTITY(1,1),
    username VARCHAR(MAX) NULL,
    password VARCHAR(MAX) NULL,
    profile_image VARCHAR(MAX) NULL,
    role VARCHAR(MAX) NULL,
    status VARCHAR(MAX) NULL,
    date_reg DATE NULL
    );


    CREATE TABLE products (
    id INT PRIMARY KEY IDENTITY(1,1),          -- Identificador único, autoincremental
    prod_id VARCHAR(50) NULL,                  -- Identificador del producto
    prod_name VARCHAR(255) NULL,               -- Nombre del producto
    prod_type VARCHAR(100) NULL,               -- Tipo del producto
    prod_stock INT NULL,                        -- Stock del producto
    prod_price FLOAT NULL,                      -- Precio del producto
    prod_status VARCHAR(50) NULL,              -- Estado del producto (ej. activo, inactivo)
    prod_image VARCHAR(255) NULL,              -- Ruta de la imagen del producto
    date_insert DATE NULL,                      -- Fecha de inserción
    date_update DATE NULL,                      -- Fecha de actualización
    date_delete DATE NULL                       -- Fecha de eliminación (para manejo de registros borrados)
    );

    CREATE TABLE orders (
    id INT PRIMARY KEY IDENTITY(1,1),           -- Identificador único, autoincremental
    customer_id INT NULL,                        -- Identificador del cliente (podría ser clave foránea)
    prod_id VARCHAR(50) NULL,                   -- Identificador del producto
    prod_name VARCHAR(255) NULL,                -- Nombre del producto
    prod_type VARCHAR(100) NULL,                -- Tipo del producto
    prod_price FLOAT NULL,                       -- Precio del producto
    order_date DATE NULL,                       -- Fecha del pedido
    delete_order DATE NULL                       -- Fecha de eliminación del pedido (para manejo de registros borrados)
    );

    ALTER TABLE orders
    ADD qty INT NULL

    CREATE TABLE customers (
    id INT PRIMARY KEY IDENTITY(1,1),        -- ID único para la tabla, se incrementa automáticamente
    customer_id INT NULL,                     -- ID del cliente, puede ser nulo
    total_price FLOAT NULL,                   -- Precio total, se debe utilizar un tipo adecuado
    date DATE NULL                            -- Fecha de la transacción, puede ser nula
    );


    ALTER TABLE customers
    ADD amount FLOAT NULL,      -- Cantidad, puede ser nula
        change FLOAT NULL;      -- Cambio, puede ser nulo

        
    INSERT INTO users (username, password, profile_image, role, status, date_reg) 
    VALUES ('admin', 'admin_password', '', 'Admin', 'Active', GETDATE());

    TRUNCATE TABLE products;
