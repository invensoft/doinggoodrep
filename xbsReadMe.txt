
// step 1: Construct the xbsPdfParamater object
XbsPdfParameter xbsPdfParamater = new XbsPdfParameter() {
                                
                                PageNumbers = pageNumber,
                                PdfFileName = pdfFile,
                                WarrantNumber = warrantNo
                            };

// step 2: Add xbsPdfParamater object to a xbsPdfParamater list
xbsPdfParameterList.Add(xbsPdfParamater);


// step 3: Pass xbsPdfParamater list object as a XbsPdfWriter class constructor //parameter
XbsPdfWriter writer = new XbsPdfWriter(xbsPdfParameterList);

// step 4: Assign the output file path to generate pdf files
writer.OutPutPath = searchedFiles;

// step 5: just call the below method.
//         it will generate warrant wise pdf from a list of input pdf files.
//         it will pick all or selected the pages in all input pdf file associated with 
//         warrant number.
  
writer.CreatePdf();


