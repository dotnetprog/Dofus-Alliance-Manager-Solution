CREATE PROCEDURE [dbo].[spSummaryReport] 
@allianceId uniqueidentifier,
@baremeAtk uniqueidentifier,
									 @baremeDef uniqueidentifier,
									 @From DateTime2,
									 @To Datetime2
AS

WITH wrapup as (SELECT m.Id AS MemberId,
       m.discordid as DiscordId,
       case 
        WHEN MAX(m.Nickname) is not null then MAX(m.Nickname) else m.alias end as Username,
       Sum(CASE
             WHEN ds.id IS NOT NULL THEN charactercount
             ELSE 0
           END)             AS Nombre_defense,
       Sum(CASE
             WHEN atks.id IS NOT NULL THEN charactercount
             ELSE 0
           END)             AS Nombre_attaques,
       Sum(CASE
             WHEN ds.id IS NOT NULL THEN ( bdefdetail.nbpepite * charactercount
                                         )
             ELSE 0
           END)             AS MontantDefPepites,
       Sum(CASE
             WHEN atks.id IS NOT NULL THEN (
             batkdetail.nbpepite * charactercount )
             ELSE 0
           END)             AS MontantAtkPepites

FROM Members m  

       Left JOIN member_screenposts msp
               ON msp.alliancememberid = m.id
       LEFT JOIN dbo.defscreens ds
              ON ds.id = msp.screenpostid
                 AND ds.statutresultatvalidation IN ( 0, 2 ) and ds.CreatedOn  >= @From
                    AND ds.CreatedOn  <  @To
   LEFT JOIN dbo.Enemies DefEnemy on DefEnemy.Id = ds.AllianceEnemyId
       LEFT JOIN dbo.atkscreens atks
              ON atks.id = msp.screenpostid
                 AND atks.statutresultatvalidation IN ( 0, 2 ) and atks.CreatedOn  >= @From
                AND atks.CreatedOn  <  @To
   LEFT JOIN dbo.Enemies AtkEnemy on AtkEnemy.Id = atks.AllianceEnemyId
       LEFT JOIN baremes bdef
              ON bdef.id =  COALESCE(DefEnemy.BaremeDefenseId,@baremeDef)
       LEFT JOIN baremedetails bdefdetail
              ON bdefdetail.baremeid = bdef.id
                 AND bdefdetail.nballie = ds.nombre_defenseur
                 AND bdefdetail.nbenemie = ds.nombre_attaquant
       LEFT JOIN baremes batk
              ON batk.id = COALESCE(AtkEnemy.BaremeAttaqueId,@baremeAtk) 
       LEFT JOIN baremedetails batkdetail
              ON batkdetail.baremeid = batk.id
                 AND batkdetail.nballie = atks.nombre_allie
                 AND batkdetail.nbenemie = atks.nombre_enemi
WHERE  m.AllianceId = @allianceId
GROUP  BY m.Id,
          m.discordid,
          m.alias),
wrapup2 as(

SELECT  w.MemberId as MemberId,
	   MAX(w.DiscordId) as DiscordId,
	   MAX(w.Username) as Username,	
	   MAX(Nombre_defense) as Nombre_defense,
	   MAX(Nombre_attaques) as Nombre_attaques,
	   MAX(MontantDefPepites) as MontantDefPepites,
	   MAX(MontantAtkPepites) as MontantAtkPepites,
	     SUM(case when avam.MontantPepites is null then 0 else avam.MontantPepites end) as MontantAvAPepites,
	COUNT(avam.Id) as NombreParticipationAvA 
		   FROM wrapup w
	   Left JOIN AvA ava on ava.AllianceId = @allianceId And ava.CreatedOn >= @From and ava.CreatedOn <  @To and ava.State = 2
	   Left JOIN AvaMembers avam on avam.MemberId = w.MemberId AND avam.AvaId = ava.Id AND (avam.ValidationState <> 2 or avam.ValidationState is null)
GROUP BY w.MemberId)

Select * from wrapup2 w2
where w2.MontantDefPepites > 0 OR w2.MontantAtkPepites > 0 Or w2.MontantAvAPepites > 0






