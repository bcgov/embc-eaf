// import { writeFile } from 'fs';
const fs = require("fs");
// Configure Angular `environment.ts` file path
const targetPath = './src/environments/environment.prod.ts';
// Load node modules
const dotenv = require('dotenv').config();

// `environment.ts` file structure
const envConfigFile = `export const environment = {
  production: true,
  apiEndpoint: '${process.env.API_URL}'
};
`;
console.log('The file `environment.ts` will be written with the following content: \n');
console.log(envConfigFile);
fs.writeFile(targetPath, envConfigFile, function (err) {
  if (err) {
    throw console.error(err);
  } else {
    console.log(`Angular environment.ts file generated correctly at ${targetPath} \n`);
  }
});
