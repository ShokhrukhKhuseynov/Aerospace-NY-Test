#Change Username and Password - put your postgres server's credentials
$env:PGPASSWORD = '123456789'
$env:PGUSERNAME = 'postgres'

    psql -U $env:PGUSERNAME -c "DROP DATABASE IF EXISTS Part"
    psql -U $env:PGUSERNAME -c "CREATE DATABASE Part;"
    psql -U $env:PGUSERNAME -d part -c "CREATE TABLE public.item
                                        (
                                            id          SERIAL PRIMARY KEY,
                                            item_name   VARCHAR(50) NOT NULL,
                                            parent_item INTEGER REFERENCES item (id),
                                            cost        INTEGER     NOT NULL,
                                            req_date    DATE        NOT NULL
                                        );"
                                        
    psql -U $env:PGUSERNAME -d part -c "BEGIN TRANSACTION;
                                        INSERT INTO public.item (item_name, parent_item, cost, req_date)
                                        VALUES ('Item1', NULL, 500, '02-20-2024'),
                                                ('Sub1', 1, 200, '02-10-2024'),
                                                ('Sub2', 1, 300, '01-05-2024'),
                                                ('Sub3', 2, 300, '01-02-2024'),
                                                ('Sub4', 2, 400, '01-02-2024'),
                                                ('Item2', NULL, 600, '03-15-2024'),
                                                ('Sub1', 6, 20, '02-25-2024');
                                        COMMIT;"
                                        
    psql -U $env:PGUSERNAME -d part -c "CREATE OR REPLACE FUNCTION Get_Total_Cost(item_name_param VARCHAR)
                                            RETURNS INTEGER AS
                                        `$BODY$
                                        DECLARE
                                            total_cost INTEGER := 0;
                                        BEGIN
                                            WITH RECURSIVE item_hierarchy AS (SELECT id, item_name, parent_item, cost
                                                                              FROM public.item
                                                                              WHERE item_name = item_name_param
                                                                                AND parent_item IS NULL
                                                                              UNION ALL
                                                                              SELECT i.id, i.item_name, i.parent_item, i.cost
                                                                              FROM public.item i
                                                                                       INNER JOIN item_hierarchy ih ON i.parent_item = ih.id)
                                            SELECT SUM(cost)
                                            INTO total_cost
                                            FROM item_hierarchy;
                                            RETURN total_cost;
                                        END;
                                        `$BODY$
                                            LANGUAGE plpgsql;"
