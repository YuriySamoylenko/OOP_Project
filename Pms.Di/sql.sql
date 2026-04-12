-- 1. Гість (тільки читання всіх таблиць)
CREATE USER 'guest'@'localhost' IDENTIFIED BY 'guest123';
GRANT SELECT ON gas_station.* TO 'guest'@'localhost';
ALTER USER 'guest'@'localhost' PASSWORD EXPIRE;

-- 2. Оператор_A (повний доступ тільки до client)
CREATE USER 'operator_A'@'localhost' IDENTIFIED BY 'operatorA123';
GRANT SELECT, INSERT, UPDATE, DELETE ON gas_station.client TO 'operator_A'@'localhost';
ALTER USER 'operator_A'@'localhost' PASSWORD EXPIRE;

-- 3. Оператор_B (повний доступ тільки до price)
CREATE USER 'operator_B'@'localhost' IDENTIFIED BY 'operatorB123';
GRANT SELECT, INSERT, UPDATE, DELETE ON gas_station.price TO 'operator_B'@'localhost';
ALTER USER 'operator_B'@'localhost' PASSWORD EXPIRE;

-- 4. Оператор_AB (повний доступ до sale + читання client і price)
CREATE USER 'operator_AB'@'localhost' IDENTIFIED BY 'operatorAB123';
GRANT SELECT, INSERT, UPDATE, DELETE ON gas_station.sale TO 'operator_AB'@'localhost';
GRANT SELECT ON gas_station.client TO 'operator_AB'@'localhost';
GRANT SELECT ON gas_station.price TO 'operator_AB'@'localhost';
ALTER USER 'operator_AB'@'localhost' PASSWORD EXPIRE;

-- Активуємо права
FLUSH PRIVILEGES;