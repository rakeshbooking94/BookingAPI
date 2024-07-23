sp_helpText GetAllHotelSearchProc


select top 50 x.bokgTransId,x.bokngResponse,x.bokngCreatedOn,y.TrnsfrTransID,x.*,y.bokngId from tblBooking  x  
inner join tblTransferBooking y 
on x.bokgTransId=y.bokgTransId 
---where y.bokngId='MAMB1TF24BK000003'
order by 1 desc

SELECT top 10
JSON_VALUE(HotelDetais, '$.id') AS HotelId,
JSON_VALUE(HotelDetais, '$.name') AS HotelName,
JSON_VALUE(HotelDetais, '$.star_rating') AS Rating,
JSON_VALUE(HotelDetais, '$.latitude') AS Latitude,
JSON_VALUE(HotelDetais, '$.longitude') AS Longitude,
JSON_VALUE(HotelDetais, '$.address') AS HotelAddress,
JSON_VALUE(HotelDetais, '$.postal_code') AS PostalCode,
JSON_VALUE(HotelDetais, '$.region.iata') AS IataCode,
JSON_VALUE(HotelDetais, '$.region.country_code') AS CountryCode,
JSON_VALUE(HotelDetais, '$.images[0]') AS Hotelimages
FROM tblRTHWKStaticData with(nolock)
WHERE ISJSON(HotelDetais) > 0   AND JSON_VALUE(HotelDetais, '$.is_closed') = 'false'
ORDER BY JSON_VALUE(HotelDetais, '$.star_rating');


select count(*) from tblRTHWKStaticData

  SELECT top 12 ID, HotelDetais
FROM tblRTHWKStaticData
FOR JSON PATH

select * from tblUser where  usrLoginId='support'

select * from tblapilog where TrackNumber='7867f5bc-7e4b-429b-ad1c-ec20448f452b'