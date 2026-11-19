CREATE DATABASE IF NOT EXISTS computer_store
  CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE computer_store;

CREATE TABLE IF NOT EXISTS categories (
  id INT NOT NULL AUTO_INCREMENT,
  name VARCHAR(100) NOT NULL,
  PRIMARY KEY (id),
  CONSTRAINT uq_categories_name UNIQUE (name)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS products (
  id INT NOT NULL AUTO_INCREMENT,
  category_id INT NOT NULL,
  name VARCHAR(150) NOT NULL,
  price DECIMAL(12,2) NOT NULL,
  stock INT NOT NULL DEFAULT 0,
  PRIMARY KEY (id),
  CONSTRAINT chk_products_price CHECK (price >= 0),
  CONSTRAINT chk_products_stock CHECK (stock >= 0),
  CONSTRAINT fk_products_category FOREIGN KEY (category_id) REFERENCES categories(id)
    ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS sales (
  id INT NOT NULL AUTO_INCREMENT,
  product_id INT NOT NULL,
  quantity INT NOT NULL,
  unit_price DECIMAL(12,2) NOT NULL,
  sold_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  CONSTRAINT chk_sales_quantity CHECK (quantity > 0),
  CONSTRAINT chk_sales_price CHECK (unit_price >= 0),
  CONSTRAINT fk_sales_product FOREIGN KEY (product_id) REFERENCES products(id)
    ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB;
