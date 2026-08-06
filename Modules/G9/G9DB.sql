

CREATE VIEW g9.v_api_deklar AS
WITH medz as (SELECT lkp_num, lkp_value FROM g9.lookup WHERE lkp_group='RuosimoMedziagos'),
	buds as (SELECT lkp_num, lkp_value FROM g9.lookup WHERE lkp_group='RuosimoBudai'),
	stat as (SELECT lkp_num, lkp_value FROM g9.lookup WHERE lkp_group='Statusas'),
	steb as (SELECT lkp_num, lkp_value FROM g9.lookup WHERE lkp_group='Stebesenos')
SELECT dkl_id "ID", dkl_gvts "GVTS", dkl_metai "Metai",
	stat.lkp_value "Statusas", steb.lkp_value "Stebesena",
	dkl_kiekis "VandensKiekis", dkl_vartot "Vartotojai",
	CASE WHEN dkl_ruosiamas THEN jsonb_build_object(
		'Medziagos',(SELECT COALESCE(jsonb_agg(lkp_value), '[]'::jsonb) FROM unnest(d.dkl_medziagos) AS id_val JOIN medz ON lkp_num = id_val), 
		'Budai', (SELECT COALESCE(jsonb_agg(lkp_value), '[]'::jsonb) FROM unnest(d.dkl_ruos_budai) AS id_val JOIN buds ON lkp_num = id_val)) ELSE null 
	END as "Ruosimas", 
	CASE WHEN COALESCE(dkl_kontaktas_vardas,dkl_kontaktas_pavarde) is null THEN null ELSE jsonb_build_object('Vardas', dkl_kontaktas_vardas, 'Pavarde', dkl_kontaktas_pavarde, 'Email', dkl_kontaktas_email, 'Phone', dkl_kontaktas_phone) END "Kontaktas",
	dkl_modif_date "Keitimas", dkl_modif_user "Keite", dkl_deklar_date "Pateiktas", dkl_deklar_user "Pateike"
FROM g9.deklaravimas d LEFT JOIN stat on (d.dkl_status=stat.lkp_num) LEFT JOIN steb on (d.dkl_stebesena=steb.lkp_num);

CREATE VIEW g9.v_api_deklar_reiksmes AS SELECT rks_id "ID", rks_deklar "Deklar", rks_rodiklis "Rodiklis", rks_date "Data", rks_reiksme "Reiksme", rks_suvedimas "Suvedimas", rks_maziau "Maziau", rks_protokolas "Protokolas" FROM g9.reiksmes 

CREATE OR REPLACE VIEW g9.v_api_ja AS
SELECT ja_id as id, ja_pavadinimas as pavad, ja_tipas as tipas, ja_statusas as statusas, 
	jsonb_build_object('aob', ja_aob, 'pavad', jad_adresas, 'kita', jad_aob) as adresas,
	CASE WHEN COALESCE(jad_kontaktas_vardas,jad_kontaktas_email,jad_kontaktas_phone) is null THEN NULL ELSE
		jsonb_build_object('vardas', jad_kontaktas_vardas, 'pavarde', jad_kontaktas_pavarde, 'email', jad_kontaktas_email, 'phone', jad_kontaktas_phone) end as kontaktas,
	jad_update pakeista
FROM g9.ja_detales WHERE jad_active;

CREATE OR REPLACE VIEW g9.v_api_gvts AS
SELECT vkl_id id, vkl_ja ja, vkl_title pavad, vkl_gvtot gvtot, jsonb_build_object('aob', vkl_adr_aob, 'pavad', vkl_adresas, 'apg', vkl_adr_apg, 'sav', vkl_adr_sav) adresas,
	vkl_active active, vkl_date pakeista FROM g9.gvts;

CREATE OR REPLACE FUNCTION g9.valid_suvesti_detales(dekl INT) RETURNS TABLE (rod_id INT, rod_kodas VARCHAR, rod_grupe VARCHAR, rod_rodiklis VARCHAR, rod_virsija INT, rod_reikia INT, rod_suvesta INT) 
LANGUAGE plpgsql AS $$ BEGIN RETURN QUERY
	WITH suv AS (SELECT * FROM g9.valid_suvesti(dekl)),
	grp AS (SELECT lkp_num, lkp_value FROM g9.lookup WHERE lkp_group = 'RodikliuGrupe'),
	rks AS (
		SELECT rks_rodiklis, max(rks_reiksme) AS rks_max, min(rks_reiksme) AS rks_min, count(*) AS rks_suvesta, bool_and(rks_maziau) AS rks_maziau
		FROM g9.reiksmes WHERE rks_deklar=dekl GROUP BY rks_rodiklis
	),
	tbl AS (
		SELECT COALESCE(rks.rks_rodiklis, suv."Rodiklis") AS rks_rodiklis, rks.rks_max, rks.rks_min, rks.rks_maziau, 
		COALESCE(rks.rks_suvesta, 0) AS rod_suvesta, COALESCE(suv."Reikia", 0) AS rod_reikia
		FROM rks FULL OUTER JOIN suv ON rks.rks_rodiklis = suv."Rodiklis"
	)
	SELECT r.rod_id, r.rod_kodas, grp.lkp_value AS rod_grupe, r.rod_rodiklis, 
		CASE 
			WHEN tbl.rks_max > r.rod_max AND tbl.rks_max > (r.rod_max + CASE WHEN COALESCE(tbl.rks_maziau, false) THEN r.rod_step ELSE 0 END) THEN 1 
			WHEN tbl.rks_min < r.rod_min THEN -1 ELSE 0 END AS rod_virsija, 
		tbl.rod_reikia, tbl.rod_suvesta::INT
	FROM tbl LEFT JOIN g9.rodikliai r ON tbl.rks_rodiklis = r.rod_id
		LEFT JOIN grp ON r.rod_grupe = grp.lkp_num;
END; $$;
