OPEN SYMMETRIC KEY SymmetricKeyCertificateClientsBankingInformationLocalAccountNumber 
DECRYPTION BY CERTIFICATE CertificateClientsBankingInformation; 

DECLARE @AccountStatusID int = [dbo].[F_PickListValues_GetPickListValueIDFromCodes]('CLIENTBANKACCOUNTSTATUS','APPROVED')
	
SELECT     
	T03_Sections.SectionCode,
	T02_Clients.ClientCode,
	CASE
		when AccountFields.Description ='AddressLine' and accountinfo.DuplicateOrder =3 then 'AddressLine3'			
		when AccountFields.Description ='AddressLine' and accountinfo.DuplicateOrder =1   then 'AddressLine1'
		when AccountFields.Description ='AddressLine' and accountinfo.DuplicateOrder =2 then 'AddressLine2'
		WHEN SectionAccountFieldDesc.Description IS NULL THEN AccountFields.Description
		ELSE SectionAccountFieldDesc.Description
	END AS Description,
	CONVERT(NVARCHAR(255), DECRYPTBYKEY(AccountInfo.Value)) Value,
	(CBI.AccountEffectiveDate),
	ACCType.Code AccountType,
	TransType.Code TransferType,
	CBI.IsActive,
	T02_Clients.UsesDirectDebit
	into #temp

FROM 
	AccountInfo 
LEFT JOIN 
	ClientsBankingInformation CBI ON CBI.ID = AccountInfo.BeneficiaryAccountID
LEFT JOIN 
	AccountFields ON AccountInfo.AccountFieldID = AccountFields.ID 
INNER JOIN 
	T02_Clients ON T02_Clients.ID = CBI.ClientsID
INNER JOIN 	
	T01_HeadOffice ON T01_HeadOffice.ID=T02_Clients.HeadOfficeID
INNER JOIN 	
	T03_Sections ON T03_Sections.ID=T02_Clients.SectionID
LEFT JOIN 
	SectionAccountFieldDesc ON SectionAccountFieldDesc.AccountFieldID=AccountInfo.AccountFieldID and SectionAccountFieldDesc.OperationID=T03_Sections.ID
LEFT JOIN
	PickListValues ACCType ON CBI.AccountType = ACCType.ID
LEFT JOIN
	PickListValues TransType ON CBI.TransferTypeID = TransType.ID
	
WHERE
	T02_Clients.DeletedDate IS NULL
	AND CBI.DeletedDate IS NULL
	AND T01_HeadOffice.ClientTypeID={0}
	AND dbo.F_IsICCSDatesValid(T01_HeadOffice.ICCSStartDate,T01_HeadOffice.ICCSEndDate,getdate())= 0
	AND T03_Sections.DeletedDate IS null	
	AND AccountEffectiveDate= (SELECT MAX(AccountEffectiveDate)
										 FROM ClientsBankingInformation
										 WHERE ClientsID = CBI.ClientsID
										 AND ClientsBankingInformation.AccountType = ACCType.ID
										-- AND AccountEffectiveDate <= @ExcecutionDate --GETDATE()
										 AND ClientsBankingInformation.DeletedDate IS NULL
										 AND ClientsBankingInformation.AccountStatusID = @AccountStatusID)
	AND CBI.AccountStatusID = @AccountStatusID

ORDER BY T03_Sections.SectionCode, T01_HeadOffice.HOCode,CBI.AccountEffectiveDate DESC				

Update #temp
set Description='Beneficiary Account'
where Description in ('BeneficiaryAccount ','[Beneficiary Account')
	

Update #temp
set Description='[Benef Bank Name]'
where Description in ('[Benef Bank Name]','[Beneficiary Bank name]')

Update #temp
set Description='Airline/Agent code'
where Description in ('Airine/ Agent Code',	'Airline/ Agent code',	'Airline/Agent code')	

Update #temp
set Description='Beneficiary Address Line'
where Description in ('Beneficiary Address Line',	'Beneficiary Address Line 1',	'Beneficiary Address Line 2',	'Beneficiary Address Line 3',	'Beneficiary Address Line1',
'Beneficiary Address Line2',	'Beneficiary Address Line3')

Update #temp
set Description='Beneficiary Account Holder'
where Description in ('Beneficiary Account Holder',	'BeneficiaryAccountHolder')

Update #temp
set Description='Beneficiary BIC'
where Description in ('Beneficiary BIC',	'BeneficiaryBIC')

Update #temp
set Description='Instructing Code'
where Description in ('Instructing Code',	'InstructionCode',	'InstructionCode_B_23E')

Update #temp
set Description='2.73 DbtrAcct Id Other Id'
where Description in ('2.73 DbtrAcct Id Other Id',	'2.73 DbtrAcct Id Othr Id')

Update #temp
set Description='2.72 Dbtr Id OrgId Other'
where Description in ('2.72 Dbtr Id OrgId Other',	'2.72 Dbtr Id OrgId Othr Identification')

Update #temp
set Description='2.72 Dbtr Id PrvtId Other'
where Description in ('2.72 Dbtr Id PrvtId Other',	'2.72 Dbtr Id PrvtId Othr Identification')

Update #temp
set Description='2.72 Dbtr Id PrvtId Othr SchemeName'
where Description in ('2.72 Dbtr Id PrvtId Othr SchemeName','2.72 Dbtr Id PrvtId Othr SchmeNm Code')

select * from #temp

drop table #temp

CLOSE SYMMETRIC KEY SymmetricKeyCertificateClientsBankingInformationLocalAccountNumber; 

