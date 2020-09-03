const fs = require("fs");
const targetPath = './src/environments/environment.prod.ts';
const dotenv = require('dotenv').config();

const envConfigFile = `export const environment = {
  production: true,
  apiEndpoint: '${process.env.API_URL}'
};
`;

fs.writeFile(targetPath, envConfigFile, function (err) {
  if (err) {
    throw console.error(err);
  } else {
    console.log(`Angular environment.prod.ts file generated correctly at ${targetPath} \n`);
  }
});
