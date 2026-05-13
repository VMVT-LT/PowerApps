SET SESSION AUTHORIZATION "Export_admin";

DROP VIEW export.v_api_cert_list

CREATE OR REPLACE VIEW export.v_api_cert_list AS
WITH cmt AS (
	SELECT DISTINCT ON (log_cert_id) log_cert_id AS log_cert, log_date, log_user_name AS log_user, log_data ->> 'text'::text AS log_comment
	FROM export.cert_log WHERE log_action::text = 'Komentaras'::text ORDER BY log_cert_id, log_date DESC
),
	kpn AS (SELECT prod_cert_id AS prod_cert, count(*) AS prod_count, jsonb_agg(prod_kpn) AS prod_kpn FROM export.cert_produktai GROUP BY prod_cert_id)
SELECT s.id cert_id, cert_nr, cert_status, cert_type, cert_isdave, cert_isdave_dep, cert_imp_salis, cert_export, cert_postas, cert_created_user_dep,
	cert_pakeistas, cert_file_count, cert_date_created, cert_blankas, cert_date_isdavimo, cert_post_issued, cert_warehouse, cert_search,
	jsonb_strip_nulls(jsonb_build_object('cert_id', s.id,
		'cert_nr', cert_nr,
		'cert_export', exp.vkl_pavad,
		'cert_imp_salis', sal.sal_pavad,
		'cert_status', cert_status,
		'cert_date_created', cert_date_created, 
		'cert_date_isdavimo', cert_date_isdavimo,
		'cert_blankas', cert_blankas,
		'cert_isdave_name', cert_isdave_name,
		'cert_rizikos_balas', cert_rizikos_balas,
		'cert_created_user_dep', cert_created_user_dep,
		'cert_request', cert_request,
		'prod_kpn', kpn.prod_kpn, 'prod_count', kpn.prod_count,
		'log_date', cmt.log_date, 'log_comment', cmt.log_comment)) data
FROM export.sertifikatai s 
	LEFT JOIN cmt ON (s.id = cmt.log_cert) 
	LEFT JOIN kpn ON (s.id = kpn.prod_cert)
	LEFT JOIN export.salys sal ON (s.cert_imp_salis = sal.id)
	LEFT JOIN export.veiklavietes exp ON (s.cert_export = exp.id)
WHERE not cert_delete;


GRANT SELECT ON TABLE export.v_api_cert_list TO export_app;



--cert_produktai
CREATE OR REPLACE VIEW export.v_api_item_cert_produktai AS
SELECT p.id id, 
	jsonb_build_object('id',p.id, 'prod_cert_id', prod_cert_id, 'prod_pavadinimas', prod_pavadinimas, 
		'gamintojas', jsonb_build_object('prod_gamintojas', g.id, 'prod_gamintojas_pavad', g.vkl_pavad), 
		'salis', jsonb_build_object('prod_salis', prod_salis, 'prod_salis_pavad', s.sal_pavad),
		'vnt', jsonb_build_object('prod_vnt', prod_vnt, 'prod_vnt_title', u.lkp_title, 'prod_vnt_descr', u.lkp_descr), 
		'kpn', jsonb_build_object('prod_kpn', prod_kpn, 'prod_last_layer', prod_last_layer, 'prod_kpn_no', COALESCE(k4.kpn_id, k3.kpn_id, k2.kpn_id, k1.kpn_id),
			'l1', jsonb_build_object('prod_l1', prod_l1, 'prod_l1_name', prod_l1_name, 'prod_l1_no', k1.kpn_no), 'l2', jsonb_build_object('prod_l2', prod_l2, 'prod_l2_name', prod_l2_name, 'prod_l2_no', k2.kpn_no),
			'l3', jsonb_build_object('prod_l3', prod_l3, 'prod_l3_name', prod_l3_name, 'prod_l3_no', k3.kpn_no), 'l4', jsonb_build_object('prod_l4', prod_l4, 'prod_l4_name', prod_l4_name, 'prod_l4_no', k4.kpn_no)
		),		
		'prod_kiekis', prod_kiekis, 'prod_kiekis_bruto', prod_kiekis_bruto, 'prod_pakuotes', prod_pakuotes, 
		'prod_kilmes_sert', prod_kilmes_sert, 'prod_bandos_nr', prod_bandos_nr,
		'rizika', COALESCE(r4.riz_balas, r3.riz_balas, r2.riz_balas, r1.riz_balas)) data
FROM export.cert_produktai p
	LEFT JOIN export.veiklavietes g ON (prod_gamintojas=g.id) 
	LEFT JOIN export.salys s ON (prod_salis=s.id) 
	LEFT JOIN export.lookup u ON (prod_vnt=u.lkp_num and u.lkp_group='prod_units')
	LEFT JOIN export.rizika_kpn r1 ON p.prod_l1 = r1.riz_kpn LEFT JOIN export.rizika_kpn r2 ON p.prod_l2 = r2.riz_kpn
	LEFT JOIN export.rizika_kpn r3 ON p.prod_l3 = r3.riz_kpn LEFT JOIN export.rizika_kpn r4 ON p.prod_l4 = r4.riz_kpn
	LEFT JOIN export.kpn_kodai k1 ON p.prod_l1 = k1.id LEFT JOIN export.kpn_kodai k2 ON p.prod_l2 = k2.id
	LEFT JOIN export.kpn_kodai k3 ON p.prod_l3 = k3.id LEFT JOIN export.kpn_kodai k4 ON p.prod_l4 = k4.id;

	
GRANT SELECT ON TABLE export.v_api_item_cert_produktai TO export_app;

--cert_produktai
CREATE OR REPLACE VIEW export.v_api_list_cert_produktai AS
SELECT prod_cert_id id, p.id sort, 
	jsonb_build_object('id',p.id, 'prod_pavadinimas', prod_pavadinimas, 
		'prod_gamintojas', g.vkl_pavad, 'prod_salis', s.sal_pavad, 'prod_kpn', prod_kpn, 'prod_kiekis', prod_kiekis,
		'prod_vnt', l.lkp_title, 'rizika', COALESCE(k4.riz_balas, k3.riz_balas, k2.riz_balas, k1.riz_balas)) data
FROM export.cert_produktai p
	LEFT JOIN export.veiklavietes g ON (prod_gamintojas=g.id) 
	LEFT JOIN export.salys s ON (prod_salis=s.id) 
	LEFT JOIN export.lookup l ON (prod_vnt=lkp_num and lkp_group='prod_units')
	LEFT JOIN export.rizika_kpn k1 ON p.prod_l1 = k1.riz_kpn
	LEFT JOIN export.rizika_kpn k2 ON p.prod_l2 = k2.riz_kpn
	LEFT JOIN export.rizika_kpn k3 ON p.prod_l3 = k3.riz_kpn
	LEFT JOIN export.rizika_kpn k4 ON p.prod_l4 = k4.riz_kpn;

--cert_kroviniai
CREATE OR REPLACE VIEW export.v_api_list_cert_kroviniai AS
SELECT krov_cert_id id, k.id sort, jsonb_build_object('id',k.id,'krov_nr',krov_nr,'krov_plomba',krov_plomba,'krov_detales',krov_detales,
	'krov_tipas_title', l.lkp_title,'krov_tipas',krov_tipas) data
FROM export.cert_kroviniai k LEFT JOIN export.lookup l ON (k.krov_tipas=lkp_num and lkp_group='cargo_type');

--cert_transportas
CREATE OR REPLACE VIEW export.v_api_list_cert_transportas AS
SELECT tran_cert_id id, t.id sort,
	jsonb_build_object('id',t.id,'tran_tipas',tran_tipas, 'tran_tipas_title', l.lkp_title, 
	'nr',tran_nr,'detales',tran_detales, 'tran_tipas_kita',tran_tipas_kita) data
FROM export.cert_transportas t LEFT JOIN export.lookup l ON (t.tran_tipas=lkp_num and lkp_group='transport_type');

--cert_users
CREATE OR REPLACE VIEW export.v_api_list_cert_users AS
SELECT user_cert id, user_owner sort,
	jsonb_build_object('user_id',c.user_id, 'user_name', user_name, 'user_mail', user_mail, 'user_owner', user_owner) data
FROM export.cert_users c LEFT JOIN export.users u ON (c.user_id=u.user_id);

--cert_log
CREATE OR REPLACE VIEW export.v_api_list_cert_log AS
SELECT log_cert_id id, l.id sort,
	jsonb_build_object('id',l.id,'title',log_title,'action',log_action,'item',l.log_item,
		'date',log_date::timestamp(0),'user',log_user,'user_name',log_user_name) data
	FROM export.cert_log l;

--cert_log_details
CREATE OR REPLACE VIEW export.v_api_list_cert_log_details AS
SELECT log_cert_id id, l.id sort,
	jsonb_build_object('id',l.id,'title',log_title,'action',log_action,'item',l.log_item,
		'date',log_date::timestamp(0),'user',log_user,'user_name',log_user_name, 
		'name',log_data->'name', 'fields',log_data->'fields') data
	FROM export.cert_log l;



GRANT SELECT ON TABLE export.v_api_list_cert_produktai TO export_app;
GRANT SELECT ON TABLE export.v_api_list_cert_kroviniai TO export_app;
GRANT SELECT ON TABLE export.v_api_list_cert_transportas TO export_app;
GRANT SELECT ON TABLE export.v_api_list_cert_users TO export_app;
GRANT SELECT ON TABLE export.v_api_list_cert_log TO export_app;
GRANT SELECT ON TABLE export.v_api_list_cert_log_details TO export_app;


  
CREATE OR REPLACE VIEW export.v_api_cert_item AS
WITH prod as (SELECT id, jsonb_agg(data) data FROM export.v_api_list_cert_produktai GROUP BY id),
	 tran as (SELECT id, jsonb_agg(data) data FROM export.v_api_list_cert_transportas GROUP BY id),
	 krov as (SELECT id, jsonb_agg(data) data FROM export.v_api_list_cert_kroviniai GROUP BY id),
	 usrs as (SELECT id, jsonb_agg(data) data FROM export.v_api_list_cert_users GROUP BY id)
SELECT s.id, cert_nr nr, 
	jsonb_build_object('cert_id', s.id, 'cert_nr', cert_nr, 'cert_status', cert_status, 'cert_blankas', cert_blankas,
		'cert_produktai', prod.data, 'cert_transportas', tran.data, 'cert_kroviniai', krov.data, 'cert_users', usrs.data,
		'cert_created', jsonb_build_object('user', cert_created_user, 'user_name', cert_created_user_name, 'user_dep', cert_created_user_dep),	
		'isdave', jsonb_build_object('cert_isdave', cert_isdave, 'cert_isdave_name', cert_isdave_name, 'cert_isdave_dep', cert_isdave_dep),
		'rizika', jsonb_build_object('cert_rizika',cert_rizika,'cert_rizika_pavad',r.riz_name,'cert_rizikos_balas',cert_rizikos_balas),
		'export', jsonb_build_object('cert_export',vke.id,'cert_export_jar',vke.vkl_jar,'cert_export_tipas',vke.vkl_tipas,'cert_export_pavad',vke.vkl_pavad,'cert_export_adresas',vke.vkl_adresas,'cert_export_rizika',cert_export_rizika),
		'import', jsonb_build_object('cert_imp_salis',cert_imp_salis, 'sal_pavad', imp.sal_pavad, 'cert_imp_gavejas', cert_imp_gavejas),
		'datos',  jsonb_build_object('cert_date_created',cert_date_created, 'cert_date_modif', cert_date_modif, 'cert_date_isdavimo', cert_date_isdavimo, 'cert_date_isvykimo', cert_date_isvykimo),
		'postas', jsonb_build_object('cert_postas', cert_postas, 'cert_postas_pavad', pst.post_pavad, 'cert_postas_kitas', cert_postas_kitas, 'cert_post_issued', cert_post_issued, 'cert_post_issued_pavad', psi.lkp_title),
		'sandel', jsonb_build_object('cert_warehouse',cert_warehouse, 'cert_warehouse_pavad', wrh.vkl_pavad),
		'cert_detales', cert_detales, 'cert_pakeistas', cert_pakeistas, 'cert_tikrinimas', cert_tikrinimas, 'cert_file_count', cert_file_count,
		'cert_delete', cert_delete, 'cert_request', cert_request, 'cert_type', cert_type, 'cert_type_pavad', tpe.lkp_title
	) data
FROM export.sertifikatai s LEFT JOIN export.veiklavietes vke ON (cert_export=vke.id) 
	LEFT JOIN prod on (s.id=prod.id) LEFT JOIN tran on (s.id=tran.id)
	LEFT JOIN krov on (s.id=krov.id) LEFT JOIN usrs on (s.id=usrs.id)
	LEFT JOIN export.rizikos r ON (r.id=cert_rizika)
	LEFT JOIN export.salys imp ON (cert_imp_salis=imp.id) 
	LEFT JOIN export.postai pst ON (cert_postas=pst.id)
	LEFT JOIN export.lookup psi ON (psi.lkp_group='post' and cert_post_issued=psi.lkp_num)
	LEFT JOIN export.lookup tpe ON (tpe.lkp_group='cert_type' and cert_type=tpe.lkp_num)
	LEFT JOIN export.veiklavietes wrh ON (cert_warehouse=wrh.id);

GRANT SELECT ON TABLE export.v_api_cert_item TO export_app;