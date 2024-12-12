INSERT INTO [Categories] ([id], [name], [parentId]) VALUES
	('4D7EAB45-6A55-46D3-B50F-0460AA9A68A7', 'Men', null),
	('87D86D61-4662-4C7A-AEB8-CE94744A60E6', 'Clothing', '4D7EAB45-6A55-46D3-B50F-0460AA9A68A7'),
	('A54CF347-6858-40C7-8951-088682581EFF', 'Accessories', '4D7EAB45-6A55-46D3-B50F-0460AA9A68A7'),
	('106F4F94-5E70-4340-B23E-462AF5FC7BFC', 'T-Shirts', '87D86D61-4662-4C7A-AEB8-CE94744A60E6'),
	('124BCF8E-83FB-4B24-A8E1-405A8E45C091', 'Pants', '87D86D61-4662-4C7A-AEB8-CE94744A60E6'),
	('4A577A76-1884-4F6A-812F-CABDDC6A5A2A', 'Jackets', '87D86D61-4662-4C7A-AEB8-CE94744A60E6'),
	('632DFDA9-CC2E-487B-8C88-608005F124E2', 'Sweaters', '87D86D61-4662-4C7A-AEB8-CE94744A60E6'),
	('77FAE516-7FE4-4752-9330-AE293FBA3705', 'Watches', 'A54CF347-6858-40C7-8951-088682581EFF'),
	('1C47AB9B-A761-4D53-8DB8-79D343BF6B8B', 'Wallets', 'A54CF347-6858-40C7-8951-088682581EFF'),
	('D003BD1D-4D11-4945-89DB-5C53306CD062', 'Sunglasses', 'A54CF347-6858-40C7-8951-088682581EFF'),
	('023B882A-0810-49C3-BAF2-AB4CE20C50B0', 'Belts', 'A54CF347-6858-40C7-8951-088682581EFF'),
	('7EC6E84A-1EF7-4A48-8FA9-40E946258216', 'Women', null),
	('BD4CD7C9-054A-41BB-8D8B-6A4E817B3E03', 'Clothing', '7EC6E84A-1EF7-4A48-8FA9-40E946258216'),
	('30300EA8-DA82-4F54-A4C8-A26D37393D38', 'Accessories', '7EC6E84A-1EF7-4A48-8FA9-40E946258216'),
	('BA5FE17F-8977-4034-BEC3-227AA99502CC', 'Tops', 'BD4CD7C9-054A-41BB-8D8B-6A4E817B3E03'),
	('35E6EF59-3419-4A43-9C23-3976722B06F9', 'Pants', 'BD4CD7C9-054A-41BB-8D8B-6A4E817B3E03'),
	('A805A418-2D53-4430-9968-D031C4F39FD4', 'T-Shirts', 'BD4CD7C9-054A-41BB-8D8B-6A4E817B3E03'),
	('050132D0-956A-496F-B9BB-B674E8015A92', 'Sweaters', 'BD4CD7C9-054A-41BB-8D8B-6A4E817B3E03'),
	('F5B601F5-302C-4D60-9465-FF96B3EA9367', 'Jackets', 'BD4CD7C9-054A-41BB-8D8B-6A4E817B3E03'),
	('4D31286C-9E2E-406A-9C01-E035E6F2A20D', 'Watches', '30300EA8-DA82-4F54-A4C8-A26D37393D38'),
	('615E2664-92E8-411B-BEF3-AE953CD325A8', 'Bags', '30300EA8-DA82-4F54-A4C8-A26D37393D38'),
	('9E6F1330-730C-47AE-B2F9-65863B5609CC', 'Sunglasses', '30300EA8-DA82-4F54-A4C8-A26D37393D38'),
	('782ECF8A-A91A-44B1-8CB4-2E482380352C', 'Hats', '30300EA8-DA82-4F54-A4C8-A26D37393D38');




INSERT INTO [Products] ([id], [name], [description], [ShortDescription], [categoryId], [sku], [stockQuantity], [unitPrice_Amount], [unitPrice_Currency], [discount], [brand], [barcode], [tags], [averageRating_Value], [averageRating_NumRatings], [CreatedOn], [Colors], [Sizes]) VALUES
	('571D3D66-18F2-4DA8-A57B-D06D4D1C9EEF', 'Black Rockstar', 'High quality black t-shirt', 'High quality black t-shirt', '106F4F94-5E70-4340-B23E-462AF5FC7BFC', 'MC-10000000T', '10', '20', '0', '10', 'Zara', 'MC-10000000T', 't-shirt, zara, men, new', '4.8', '5', GETDATE(), 'Black,Gray,Orange,Pink', 'XS,S,M,L,XL,2XL'),
	('096A127A-65AF-4C0C-A294-2E1990136711', 'Premium Purple', 'High quality purple t-shirt', 'High quality purple t-shirt', '106F4F94-5E70-4340-B23E-462AF5FC7BFC', 'MC-10000002T', '5', '22', '0', '10', 'ck', 'MC-10000002T', 't-shirt, ck, men, new, premium', '4.9', '7', GETDATE(), 'Black,Orange,Pink,White', 'XS,S,M,L,XL,2XL'),
	('8419FD69-61A5-4D9D-A289-1E377CC17082', 'Red Dragon', 'Premium quality red sweater', 'Premium quality red sweater', '632DFDA9-CC2E-487B-8C88-608005F124E2', 'MC-10000000S', '20', '40', '0', '10', 'h&m', 'MC-10000000S', 'sweater, h&m, men, hot, premium', '4.7', '15', GETDATE(), 'Red,Orange,Pink,White', 'XS,S,M,L,XL,2XL'),
	('CBB8A3A9-03DB-40E4-A89B-9C91D21B3934', 'Ash life', 'High quality t-shirt', 'High quality t-shirt', '106F4F94-5E70-4340-B23E-462AF5FC7BFC', 'MC-10000004T', '15', '25', '0', '10', 'ck', 'MC-10000004T', 't-shirt, ck, men, new, premium', '4.7', '5', GETDATE(), 'Black,Gray,Pink,White', 'XS,S,M,L,XL,2XL'),
	('96A1503F-B7EA-4C12-8946-89035399F45E', 'Red Star', 'High quality t-shirt for men', 'High quality t-shirt for men', '106F4F94-5E70-4340-B23E-462AF5FC7BFC', 'MC-10000006T', '30', '20', '0', '10', 'Boss', 'MC-10000006T', 't-shirt, boss, men', '4.6', '10', GETDATE(), 'Red,Black,Orange', 'XS,S,M,L,XL,2XL'),
	('E02500EA-2957-4601-A051-951CB9957CBE', 'Black Stone', 'Premium quality t-shirt for women', 'Premium quality t-shirt for women', 'A805A418-2D53-4430-9968-D031C4F39FD4', 'FC-10000000T', '30', '40', '0', '10', 'Adidas', 'FC-10000000T', 't-shirt, adidas, new, premium', '4.9', '15', GETDATE(), 'Black,Gray', 'XS,S,M,L,XL,2XL'),
	('A34BA283-62A1-43C6-8FB0-748DBDE8BA7D', 'Elegant peach', 'High quality women top', 'High quality women top', 'BA5FE17F-8977-4034-BEC3-227AA99502CC', 'FC-10000002T', '10', '20', '0', '10', 'h&m', 'FC-10000002T', 'top, h&m, new', '4.7', '15', GETDATE(), ',Orange,Pink,Peach', 'XS,S,M,L,XL,2XL'),
	('D7C96816-1656-4DBC-B709-0F626807347F', 'Pure White', 'High quality women t-shirt', 'High quality women t-shirt', 'A805A418-2D53-4430-9968-D031C4F39FD4', 'FC-10000004T', '20', '20', '0', '10', 'ck', 'FC-10000004T', 't-shirt, ck, new', '4.7', '5', GETDATE(), 'White', 'XS,S,M,L,XL,2XL'),
	('DF22BA6B-A15B-4125-AF29-07E242AF6F2D', 'Natural Green', 'High quality t-shirt', 'High quality t-shirt', 'A805A418-2D53-4430-9968-D031C4F39FD4', 'FC-10000006T', '20', '25', '0', '5', 'Zara', 'FC-10000006T', 't-shirt, zara', '4.6', '5', GETDATE(), 'Green', 'XS,S,M,L,XL,2XL'),
	('9EB9982F-E672-4184-8399-044277EEF754', 'New Way', 'Premium quality top', 'Premium quality top', 'BA5FE17F-8977-4034-BEC3-227AA99502CC', 'FC-10000008T', '30', '30', '0', '10', 'Nike', 'FC-10000008T', 'top, nike, new', '4.8', '20', GETDATE(), 'Black,Gray,Orange,Pink,White', 'XS,S,M,L,XL,2XL');


INSERT INTO [ProductImages] ([id], [productId], [imageUrl], [seq], [publicId]) VALUES
	('4DBB9D7F-5B09-4ECF-AEB9-B1F5F6473379', '571D3D66-18F2-4DA8-A57B-D06D4D1C9EEF', 'https://res.cloudinary.com/akazad13/image/upload/v1729420812/shopizy/mpjks43jpxo9eagempgu.jpg', '0', 'mpjks43jpxo9eagempgu'),
	('4CD04D71-3249-4AE3-B079-5369C55ABB48', '096A127A-65AF-4C0C-A294-2E1990136711', 'https://res.cloudinary.com/akazad13/image/upload/v1729420811/shopizy/izygsyuzqsd71ricsyon.jpg', '0', 'izygsyuzqsd71ricsyon'),
	('0D46C06C-12FB-42C1-B0CA-86E58226ED8F', '8419FD69-61A5-4D9D-A289-1E377CC17082', 'https://res.cloudinary.com/akazad13/image/upload/v1729420811/shopizy/soy4l9t0xba9dz2aq3li.jpg', '0', 'soy4l9t0xba9dz2aq3li'),
	('D6FAC361-FA14-4E47-9B90-5C8C4EBA6762', 'CBB8A3A9-03DB-40E4-A89B-9C91D21B3934', 'https://res.cloudinary.com/akazad13/image/upload/v1729420811/shopizy/uxix3peu5uhvzai2n8do.jpg', '0', 'uxix3peu5uhvzai2n8do'),
	('26BBDC5E-270A-45F0-9AC8-2B4CC28C2126', '96A1503F-B7EA-4C12-8946-89035399F45E', 'https://res.cloudinary.com/akazad13/image/upload/v1729420811/shopizy/rbzoctialordszafqium.jpg', '0', 'rbzoctialordszafqium'),
	('91A192FC-81A4-4BF0-9704-97630689F461', 'E02500EA-2957-4601-A051-951CB9957CBE', 'https://res.cloudinary.com/akazad13/image/upload/v1729420810/shopizy/i7rpwhzj6iqthgbkfftr.jpg', '0', 'i7rpwhzj6iqthgbkfftr'),
	('44868001-5253-4A64-9CA3-039CD129ED05', 'A34BA283-62A1-43C6-8FB0-748DBDE8BA7D', 'https://res.cloudinary.com/akazad13/image/upload/v1729420811/shopizy/dhpe8da07epnawtqftof.jpg', '0', 'dhpe8da07epnawtqftof'),
	('27332C6A-AFDC-4586-A83B-915707DE419F', 'D7C96816-1656-4DBC-B709-0F626807347F', 'https://res.cloudinary.com/akazad13/image/upload/v1729420811/shopizy/awjhedzbdirmtve1ukj6.jpg', '0', 'awjhedzbdirmtve1ukj6'),
	('D3E23931-88E9-4E0E-9A61-77BFC02FA964', 'DF22BA6B-A15B-4125-AF29-07E242AF6F2D', 'https://res.cloudinary.com/akazad13/image/upload/v1729420811/shopizy/laeklvddgoewdct3hrly.jpg', '0', 'laeklvddgoewdct3hrly'),
	('D3E23931-88E9-4E0E-9A61-77BFC02FA964', '9EB9982F-E672-4184-8399-044277EEF754', 'https://res.cloudinary.com/akazad13/image/upload/v1729420812/shopizy/ojtymvp04czf75leclg2.jpg', '0', 'ojtymvp04czf75leclg2'),
    ('A8A70F86-7987-4BB4-A344-1B2133DCC609', '096A127A-65AF-4C0C-A294-2E1990136711', 'https://res.cloudinary.com/akazad13/image/upload/v1733579506/shopizy/men2_2_d4dn3p.jpg', 1, 'men2_2_d4dn3p');

INSERT INTO ProductReviews(Id, UserId, ProductId, Rating_Value, Comment, CreatedOn)
VALUES('20AB2C8F-E880-45C3-8887-D0A0E2E0E763','E68D8E76-72A1-42ED-91D0-0ED0296D662E','DF22BA6B-A15B-4125-AF29-07E242AF6F2D',5, 'Looks Awesome!', GETDATE()),
('C75B65D5-13D1-4FC9-A81E-19D7FB3B7784','67726B00-2647-4817-BFCB-3781EBD97E0F','DF22BA6B-A15B-4125-AF29-07E242AF6F2D',5, 'Looks Awesome!', GETDATE()),
('41B53394-5014-45E6-9540-B7FA05782B6C','6EBA643C-D138-4D90-AA7C-6DBF935D8209','DF22BA6B-A15B-4125-AF29-07E242AF6F2D',4, 'Good T-Shirt!', GETDATE()),
('F9CA8F97-2B91-4EAD-88D7-78496A92616A','88D9AF82-BEF4-4042-8E55-B01B89F23E68','DF22BA6B-A15B-4125-AF29-07E242AF6F2D',5, 'Recommanded!', GETDATE()),
('7ED64773-2ED6-4BFB-BBA0-E267BA4A8208','E68D8E76-72A1-42ED-91D0-0ED0296D662E','DF22BA6B-A15B-4125-AF29-07E242AF6F2D',4, 'Looks good', GETDATE());
