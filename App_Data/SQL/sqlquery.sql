﻿sp_helpText GetAllHotelSearchProc


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

202.143.99.106

820839	test_hotel	Belogorsk	RU	{"address":"123 Moscow street, Belogorsk","amenity_groups":[{"amenities":["Air conditioning","Elevator/lift","Smoking areas","Heating","Security guard","Terrace","Fire Extinguisher","Smoking Allowed","Electric car charging"],"group_name":"General"},{"amenities":["Fridge","Cable TV","TV","Music system","Minibar","DVD Player","Locker","Bathrobe","Safe (in room)"],"group_name":"Rooms"},{"amenities":["Accessibility features"],"group_name":"Accessibility"},{"amenities":["Iron and board","Hairdryer (on request)","Telephone","Iron"],"group_name":"Services and amenities"},{"amenities":["Common kitchen","Kitchen"],"group_name":"Meals"},{"amenities":["Wi-Fi"],"group_name":"Internet"},{"amenities":["Russian","English"],"group_name":"Languages Spoken"},{"amenities":["Sun Deck"],"group_name":"Recreation"},{"amenities":["Parking"],"group_name":"Parking"},{"amenities":["Photocopy machines"],"group_name":"Business"},{"amenities":["First Aid Kit"],"group_name":"Beauty and wellness"},{"amenities":["Kids' TV Networks"],"group_name":"Kids"},{"amenities":["Pets allowed"],"group_name":"Pets"},{"amenities":["Temperature control for staff"],"group_name":"Health and Safety Measures"}],"check_in_time":"14:00:00","check_out_time":"12:00:00","description_struct":[{"paragraphs":["A very nice place to stay for a reasonable price — living quarters «Test Hotel (Do Not Book)» are located in Belogorsk. These living quarters are located in walking distance from the city center."],"title":"Location"},{"paragraphs":["In the shared kitchen, you can cook anything you want. Wi-Fi is available on the premises. Ask for more information when checking-in. Specially for tourists who travel by car, there’s a parking zone. Pets are welcome here.","Accessible for guests with disabilities: the elevator helps them to go to the highest floors. Additional services that the living quarters offers to its guests: ironing. The staff of the living quarters will be happy to talk to you in English and Russian."],"title":"At the living quarters"},{"paragraphs":["The room is warmly decorated and has everything you need to have a rest after a long eventful day. There is a DVD player, a TV, a mini-bar and a bathrobe. The room equipment depends on its category."],"title":"Room amenities"}],"id":"test_hotel","images":["https://cdn.worldota.net/t/{size}/extranet/b6/32/b63252ca248284f1d76a04f687148dd842105c32.jpeg","https://cdn.worldota.net/t/{size}/extranet/38/da/38da64833864125e6bce2c5ce3693d08d17b58dd.jpeg","https://cdn.worldota.net/t/{size}/extranet/a1/c4/a1c43714db4b446d8cb45343ee901a688b376871.jpeg","https://cdn.worldota.net/t/{size}/extranet/34/dd/34ddb81b601a3ed939dbda64726503286caab75a.jpeg","https://cdn.worldota.net/t/{size}/extranet/3f/ac/3fac1f63d1c0f5e3929e6a73abf074aec400b6fe.jpeg","https://cdn.worldota.net/t/{size}/extranet/8a/84/8a8406f0a7c9bd1f96c79383b44e927c114f136d.jpeg","https://cdn.worldota.net/t/{size}/extranet/03/9d/039d14e20ed3f1271d98acb2bf3bc3fee973ff89.jpeg","https://cdn.worldota.net/t/{size}/extranet/12/48/124805d0eebf5672e92fef123f8526d714c27487.jpeg","https://cdn.worldota.net/t/{size}/extranet/01/88/01887841dc7e13d90860e5cb618283dcd1ff4aa5.jpeg"],"kind":"Hotel","latitude":50.91801834106445,"longitude":128.487060546875,"name":"Test Hotel (Do Not Book)","phone":"+0011111","policy_struct":[{"paragraphs":["Price of an additional breakfast: 500.00 RUB per person. Information about the type of meals included in the price is indicated in the rate details."],"title":"Meals"},{"paragraphs":["Fee for an extra bed: 100.00 ALL per night.","The number of extra beds depends on the room category. You must take a look at the information about the size of the selected room.","Breakfast for children aged 0-5 costs: 300.00 RUB.","Breakfast for children aged 6-12 costs: 400.00 RUB.","Children from the age of 0 to 3 can stay in additional beds (child bed/cot) for a charge of 500.00 CHF a night providing they share a room with their parents or guardians. Additional beds (child bed/cot) available on request.","Children from the age of 4 to 9 can stay in additional beds (child bed/cot) for a charge of 700.00 ALL a night providing they share a room with their parents or guardians. Additional beds (child bed/cot) available on request.","Children from the age of 10 to 12 can stay free of charge providing they share a room with their parents or guardians, in the existing beds."],"title":"Children and information about extra beds"},{"paragraphs":["Deposit is required. Cost: 100.00 RUB per room per stay.","You can request the documents necessary for a visa, the service is provided for an additional fee. Any additional fee will have to be paid even if the booking is subsequently cancelled, and all the agreements exist exclusively between you and the provider of the documents.","Attention! If you do not check in to your room before 9:00 PM at night, the booking will be cancelled."],"title":"Special living conditions"},{"paragraphs":["Pets are allowed for an additional fee. Price of accommodation: 50.00 EUR per stay."],"title":"Pets"},{"paragraphs":["Chargeable parking available. Cost: 500.00 RUB per day."],"title":"Parking space"},{"paragraphs":["Transfer from/to railway station available. Cost: 1,000.00 RUB one way.","Transfer from/to airport available. Cost: 2,000.00 RUB one way."],"title":"Transfer"},{"paragraphs":["Russian citizens must have an original Russian passport upon arrival.","The given hotel is fiction and should not be booked. If you book this hotel further accommodation shall not be provided.\r","\r","The front desk is not open around o'clock. If the guests arrive after 09:00 РM, it is necessary to notify the hotel in advance and get the check-in instructions. Contact information is provided in the booking confirmation.\r","\r","The hotel charges a resort fee of AED 15.00 per room for one night, for a two-room — AED 30.00 per day, for a three-room — AED 45.00 per day."],"title":"Extra info"}],"postal_code":null,"room_groups":[{"room_group_id":90,"images":["https://cdn.worldota.net/t/{size}/extranet/12/cf/12cfca71cf812714e6dd0ea6ec78ec3b22383808.jpeg","https://cdn.worldota.net/t/{size}/extranet/98/82/9882ec59804ceb320083254c1a3e36fcea5254a0.jpeg","https://cdn.worldota.net/t/{size}/extranet/58/cd/58cd094c3a4c39de80b0321d219a58164caad35e.png"],"name":"Standard Double room full double bed","room_amenities":["barbecue","fitness","golf","private-bathroom"],"rg_ext":{"class":3,"quality":2,"sex":0,"bathroom":2,"bedding":3,"family":0,"capacity":2,"club":0,"bedrooms":0,"balcony":0,"floor":0,"view":0},"name_struct":{"bathroom":null,"bedding_type":"full double bed","main_name":"Standard Double room"}},{"room_group_id":37368372,"images":[],"name":"Standard Plus Double room full double bed","room_amenities":["private-bathroom"],"rg_ext":{"class":3,"quality":2,"sex":0,"bathroom":2,"bedding":3,"family":0,"capacity":2,"club":1,"bedrooms":0,"balcony":0,"floor":0,"view":0},"name_struct":{"bathroom":null,"bedding_type":"full double bed","main_name":"Standard Plus Double room"}},{"room_group_id":18396068,"images":[],"name":"Family room","room_amenities":["private-bathroom"],"rg_ext":{"class":3,"quality":25,"sex":0,"bathroom":2,"bedding":0,"family":0,"capacity":0,"club":0,"bedrooms":0,"balcony":0,"floor":0,"view":0},"name_struct":{"bathroom":null,"bedding_type":null,"main_name":"Family room"}},{"room_group_id":35173284,"images":[],"name":"Family Plus room","room_amenities":["private-bathroom"],"rg_ext":{"class":3,"quality":25,"sex":0,"bathroom":2,"bedding":0,"family":0,"capacity":0,"club":1,"bedrooms":0,"balcony":0,"floor":0,"view":0},"name_struct":{"bathroom":null,"bedding_type":null,"main_name":"Family Plus room"}},{"room_group_id":18395671,"images":["https://cdn.worldota.net/t/{size}/extranet/f5/f8/f5f8ad26819a471318d24631fa5055036712a87e.jpeg","https://cdn.worldota.net/t/{size}/extranet/30/42/30420d1a9afb2bcb60335812569af4435a59ce17.jpeg","https://cdn.worldota.net/t/{size}/extranet/9c/3d/9c3dcb1f9185a314ea25d51aed3b5881b32f420c.jpeg","https://cdn.worldota.net/t/{size}/extranet/1b/46/1b4605b0e20ceccf91aa278d10e81fad64e24e27.jpeg"],"name":"1 Bedroom Apartment","room_amenities":["barbecue","bath","bathrobe","kitchen","private-bathroom","safe"],"rg_ext":{"class":6,"quality":0,"sex":0,"bathroom":2,"bedding":0,"family":0,"capacity":0,"club":0,"bedrooms":1,"balcony":0,"floor":0,"view":0},"name_struct":{"bathroom":null,"bedding_type":null,"main_name":"1 Bedroom Apartment"}}],"region":{"id":6308866,"country_code":"RU","iata":null,"name":"Belogorsk","type":"City"},"star_rating":0,"email":"10@list.ru","serp_filters":["has_internet","has_parking","has_kids","has_disabled_support","air_conditioning","has_pets","kitchen","has_smoking","has_ecar_charger"],"is_closed":true,"is_gender_specification_required":true,"metapolicy_struct":{"internet":[{"inclusion":"included","price":"0.00","currency":null,"price_unit":"unspecified","internet_type":"unspecified","work_area":"hotel"},{"inclusion":"not_included","price":"35.00","currency":"RUB","price_unit":"per_hour","internet_type":"unspecified","work_area":"room"}],"meal":[{"inclusion":"not_included","price":"500.00","currency":"RUB","meal_type":"breakfast"}],"children_meal":[{"inclusion":"not_included","price":"300.00","currency":"RUB","meal_type":"breakfast","age_start":0,"age_end":5},{"inclusion":"not_included","price":"400.00","currency":"RUB","meal_type":"breakfast","age_start":6,"age_end":12}],"extra_bed":[{"inclusion":"not_included","price":"100.00","currency":"ALL","price_unit":"per_guest_per_night","amount":1}],"cot":[{"inclusion":"included","price":"0.00","currency":null,"price_unit":"unspecified","amount":1}],"pets":[{"inclusion":"not_included","price":"50.00","currency":"EUR","price_unit":"per_guest_per_stay","pets_type":"unspecified"}],"shuttle":[{"inclusion":"not_included","price":"1000.00","currency":"RUB","shuttle_type":"one_way","destination_type":"train"},{"inclusion":"not_included","price":"2000.00","currency":"RUB","shuttle_type":"one_way","destination_type":"airport"},{"inclusion":"included","price":"0.00","currency":null,"shuttle_type":"unspecified","destination_type":"airport_train"}],"parking":[{"inclusion":"not_included","price":"500.00","currency":"RUB","price_unit":"per_guest_per_night","territory_type":"unspecified"}],"children":[{"age_start":0,"age_end":3,"price":"500.00","currency":"CHF","extra_bed":"available"},{"age_start":4,"age_end":9,"price":"700.00","currency":"ALL","extra_bed":"available"},{"age_start":10,"age_end":12,"price":"0.00","currency":null,"extra_bed":"available"}],"visa":{"visa_support":"support_enable"},"deposit":[{"availability":"available","price":"1000.00","currency":"RUB","price_unit":"per_room_per_stay","pricing_method":"fixed","payment_type":"card","deposit_type":"keys"},{"availability":"available","price":"10000.00","currency":"RUB","price_unit":"per_room_per_stay","pricing_method":"fixed","payment_type":"cash","deposit_type":"pet"},{"availability":"available","price":"500.00","currency":"RUB","price_unit":"per_room_per_stay","pricing_method":"fixed","payment_type":"unspecified","deposit_type":"breakage"},{"availability":"available","price":"100.00","currency":"RUB","price_unit":"per_room_per_stay","pricing_method":"fixed","payment_type":"unspecified","deposit_type":"unspecified"}],"no_show":{"availability":"available","time":"09:00:00","day_period":"after_midday"},"add_fee":[{"price":"100.00","currency":"RUB","price_unit":"per_guest_per_night","fee_type":"television"},{"price":"450.00","currency":"RUB","price_unit":"per_guest_per_stay","fee_type":"bed_linen"},{"price":"300.00","currency":"RUB","price_unit":"per_guest_per_stay","fee_type":"towels_only"},{"price":"200.00","currency":"RUB","price_unit":"per_room_per_night","fee_type":"conditioning"},{"price":"300.00","currency":"RUB","price_unit":"per_room_per_night","fee_type":"refrigerator"},{"price":"100.00","currency":"RUB","price_unit":"per_guest_per_night","fee_type":"microwave"},{"price":"700.00","currency":"RUB","price_unit":"per_guest_per_night","fee_type":"towels"}],"check_in_check_out":[]},"metapolicy_extra_info":"Russian citizens must have an original Russian passport upon arrival.\nThe given hotel is fiction and should not be booked. If you book this hotel further accommodation shall not be provided.\r\n\r\nThe front desk is not open around o'clock. If the guests arrive after 09:00 РM, it is necessary to notify the hotel in advance and get the check-in instructions. Contact information is provided in the booking confirmation.\r\n\r\nThe hotel charges a resort fee of AED 15.00 per room for one night, for a two-room — AED 30.00 per day, for a three-room — AED 45.00 per day.","star_certificate":null,"facts":{"floors_number":2,"rooms_number":11,"year_built":2000,"year_renovated":2021,"electricity":{"frequency":[50],"voltage":[220],"sockets":["c","f"]}},"payment_methods":["visa","euro_master_card","mir"],"hotel_chain":"No chain","front_desk_time_start":"08:00:00","front_desk_time_end":"00:00:00","semantic_version":0}	2024-07-18 17:34:07.587